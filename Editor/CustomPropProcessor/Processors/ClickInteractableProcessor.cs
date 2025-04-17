using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Assertions;
using DuksGames.Argon.Core;
using System.Linq;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Shared;
using DuksGames.Argon.Event;
using UnityEditor.Events;
using System;
using DuksGames.Argon.Interaction;

namespace DuksGames.Tools
{
    public enum SignalFilterModifierType
    {
        DontFilter, ConstantValue, OneMinusSignal
    }

    public class ClickInteractableProcessorKeySet : AbstractCustomPropKeySet<ClickInteractableProcessor>
    {
        public override string TargetKey => "mel_interaction_handler";

        public override IEnumerable<string> GetKeys()
        {
            yield return this.AppendSuffix("_playable");

            yield return this.AppendSuffix("_num_extra_playables");
            yield return this.AppendSuffix("_playable1");
            yield return this.AppendSuffix("_playable2");
            yield return this.AppendSuffix("_playable3");
            yield return this.AppendSuffix("_playable4");
            yield return this.AppendSuffix("_playable5");

            yield return this.AppendSuffix("_has_toggle_modifier");
            yield return this.AppendSuffix("_interaction_type");
            yield return this.AppendSuffix("_is_trigger_enter_exit");

            yield return this.AppendSuffix("_should_forward_interactions");
            yield return this.AppendSuffix("_scene_object_forward_target");

            // Enter / exit signal provider
            yield return this.AppendSuffix("_enter_signal_input_type");
            yield return this.AppendSuffix("_exit_signal_input_type");
            yield return this.AppendSuffix("_enter_signal_provider");
            yield return this.AppendSuffix("_exit_signal_provider");
            yield return this.AppendSuffix("_enter_signal_playable");
            yield return this.AppendSuffix("_exit_signal_playable");
            yield return this.AppendSuffix("_enter_signal");
            yield return this.AppendSuffix("_exit_signal");

            yield return this.AppendSuffix("_is_click_hold");
            yield return this.AppendSuffix("_handle_discrete_clicks_also");
            yield return this.AppendSuffix("_discrete_click_fake_hold_time");

            yield return this.AppendSuffix("_click_feedback");

            // self desctruct 
            yield return this.AppendSuffix("_self_destruct_behaviour");
            yield return this.AppendSuffix("_destroy_highlighter_also");
            yield return this.AppendSuffix("_destroy_collider_also");
            yield return this.AppendSuffix("_destroy_game_object_also");

            // sleep behaviour
            yield return this.AppendSuffix("_sleep_behaviour");

            // initial sleep stat
            yield return this.AppendSuffix("_set_initial_sleep_state");

            // sleep spreader 
            yield return this.AppendSuffix("_sleep_highlighter_also");
            yield return this.AppendSuffix("_sleep_collider_also");

            yield return this.AppendSuffix("_sleep_highlighter_during_execution");

        }
    }

    public class ClickInteractableProcessor : AbstractCustomPropProcessor, IIntermediateProductConsumer, ILinkerProcess
    {

        AbstractCommand GetCommand(string playableId, IntermediateProductSet intermediateProductSet)
        {

            var command = CommandFactory.FindCommand(intermediateProductSet, playableId);

            if (command == null)
            {
                Debug.LogWarning($"ARGON: no command found for interactable on {this.ApplyInfo.Target.name}. Looking for a playable named: '{playableId}' ");
                return null;
            }

            return command;
        }

        public void Consume(ModelPostProcessInfo mppi, IntermediateProductSet intermediateProductSet)
        {
            var playableId = this.GetStringWithSuffix("_playable");
            var playableKeys = Enumerable.Range(0, this.GetIntWithSuffix("_num_extra_playables") + 1).Select(i => i == 0 ? "_playable" : $"_playable{i}");

            var playableIds = playableKeys.Select(key =>
            {
                var pId = this.GetStringWithSuffix(key);
                if (pId == null) { Debug.LogWarning($"null pId for key {key} on target: {this.ApplyInfo.Target.name}"); }
                return pId;
            }).Where(pId => pId != null);


            // TODO: issue a warning if we get any null pIDs
            var commandsList = playableIds.Select(pId => this.GetCommand(pId, intermediateProductSet)).ToList();
            commandsList.RemoveAll(cmd => cmd == null);
            var commands = commandsList.ToArray();

            Assert.IsTrue(commands is IExecute[], $"Need these commands to be IExecute types. Instead got the following types: {commands.Select(cmd => cmd.GetType().ToString())} ");

            int interactionType = this.GetValueWithSuffix<int>("_interaction_type", true); // 0 click , 1 trigger

            var signalProvider = this.CreateSignalSource(mppi, intermediateProductSet);

            MonoBehaviour handler = null;

            if (interactionType == 1)
            { // trigger interaction
                var triggerHandler = this.ApplyInfo.Target.AddComponent<TriggerInteractionHandler>();

                triggerHandler.EnterExit = (TriggerInteractionHandler.EnterExitHandling)this.GetIntWithSuffix("_is_trigger_enter_exit");

                triggerHandler.IExecuteLinks = commands;
                triggerHandler.ICommandSignalSourceLink = signalProvider;

                handler = triggerHandler;
            }
            else
            { // interactionType == 0  // click interaction

                var interactionHandler = this.GetBoolWithSuffix("_is_click_hold", true) ?
                    this.ApplyInfo.Target.AddComponent<ClickHoldInteractionHandler>() :
                    this.ApplyInfo.Target.AddComponent<ClickInteractionHandler>();

                if (interactionHandler is ClickHoldInteractionHandler clickHoldHandler)
                {
                    clickHoldHandler.HandleDiscreteClicksAlso = this.GetBoolWithSuffix("_handle_discrete_clicks_also", true);
                    clickHoldHandler.DiscreteClickFakeHoldTime = this.GetFloatWithSuffix("_discrete_click_fake_hold_time");
                }

                interactionHandler.IExecuteLinks = commands;
                interactionHandler.ICommandSignalSourceLink = signalProvider;

                if (this.GetBoolWithSuffix("_click_feedback", true))
                {
                    throw new System.Exception("Not implementing click feedback atm. ");
                }

                if (this.GetIntWithSuffix("_sleep_behaviour") == 1) // after any interaction
                {
                    var sleeper = interactionHandler.gameObject.AddComponent<InteractionSleeper>();
                    sleeper.target = interactionHandler;
                }

                if(this.GetBoolWithSuffix("_should_forward_interactions", true))
                {
                    this.SetupForwardInteractions(interactionHandler);
                }

                handler = interactionHandler;
            }

            SetupSleepStateSetter.Setup(handler, this.ApplyInfo.Target, s => this.GetIntWithSuffix(s));

        }

        void SetupForwardInteractions(ClickInteractionHandler interactionHandler)
        {
            var forwardTargetName = this.GetStringWithSuffix("_scene_object_forward_target");
            var forwardObject = forwardTargetName == ArgonImportSymbols.ThisObject ? 
                                interactionHandler.gameObject :
                                ShGameObjectHelper.FindInRoot(interactionHandler.transform, forwardTargetName)?.gameObject;
            if(forwardObject != null) // forwardTargetName == ArgonImportSymbols.ThisObject)
            {
                // Logger.Log($"find forw ob: '{forwardObject.name}'");
                try {
                    // Try to find a click interaction handler on the forward object
                    //   it might not have beed added yet. 
                    // var DCompos = forwardObject.GetComponents<Component>();
                    // Logger.Log($"found {DCompos.Length} components");
                    var cih = forwardObject.GetComponents<Component>().First(h => h is IClickInteractionHandler && h != interactionHandler);
                    UnityEventTools.AddPersistentListener(interactionHandler.OnInteracted, ((IClickInteractionHandler) cih).Interact);
                    return;
                } 
                catch (System.InvalidOperationException) 
                {
                    // Logger.Log($"There was a prob so we'll be adding a self forwarder: {e}");
                    var handler = forwardObject.AddComponent<InteractionSelfForwarder>(); 
                                    // 
                    Assert.IsFalse(handler == null, $"didn't find an IClickInteractionHandler on object named '{forwardTargetName}'");
                    UnityEventTools.AddPersistentListener(interactionHandler.OnInteracted, handler.HandleInteraction);
                }

                // if the __THIS_OBJECT__ case use an InteractionSelfForwarder instead
                // var selfForwarder = interactionHandler.gameObject.AddComponent<InteractionSelfForwarder>();
                // UnityEventTools.AddPersistentListener(interactionHandler.OnInteracted, selfForwarder.HandleInteraction);
                return;
            }

            // TODO: never use these Assign Field From Scene objects things. Strive to never require an extra set up step
            //   beyond right-click > reimport on the fbx.

            // TODO: eliminate command signal source when its just a zero / one 

            // CONSIDER: we could short cut through these classes if we just want to forward interactions
            //   to this object. Keeping for now because its simple but flexible on the configure side.
            // TODO: find ways to reduce the number of components. Sometimes we add so many that its pretty overwhelming
            //   and they are all just finding objects that are actually just the object they're attached to...
            //   Besides...If we are able to wire up the forwarding right here on this object or this hierarchy
            //     it saves the user from having to have an override on their objects in scene (and from having to run the 
            //       wire-up function)

            if (!string.IsNullOrEmpty(forwardTargetName))
            {
                Logger.Log($"Add a scene ob referencer".Pink());
                // TODO: if existing referencer...
                // Don't make the user add a separate SceneObjectReferencerLike in blender
                //   its too complicated.
                var referencer = interactionHandler.gameObject.AddComponent<SceneObjectsReferencer>();
                referencer.Targets = new string[] { forwardTargetName };
            }

            var forwarder = interactionHandler.gameObject.AddComponent<InteractionForwarder>();
            forwarder.Referencer = interactionHandler.GetComponent<SceneObjectsReferencer>();
            UnityEventTools.AddPersistentListener(interactionHandler.OnInteracted, forwarder.HandleInteraction);
        }

        // private void AddHighlighterSleeping(List<AbstractCommand> commandsList, ModelPostProcessInfo mppi)
        // {
        //     // WANT but test
        //     // if(!this.GetBoolWithSuffix("_sleep_highlighter_during_execution", true)) {
        //     //     return;
        //     // }
        //     // TODO: let users opt in to highlighter sleeping.
        //     if (mppi.AssociateLookup.Get(this.ApplyInfo.Target).Highlighter == null) {
        //         return;
        //     }
        //     var highlighterSleeper = this.ApplyInfo.Target.AddComponent<SleeperCommandsListener>();
        //     highlighterSleeper.Commands = commandsList.ToArray();
        //     // highlighterSleeper.ISleepLinks = new Component[] {
        //     //     mppi.AssociateLookup.Get(this.ApplyInfo.Target).Highlighter
        //     // };
        //     highlighterSleeper.Highlighters = new AbstractHighlighter[] {
        //         mppi.AssociateLookup.Get(this.ApplyInfo.Target).Highlighter
        //     };
        // }

        CommandSignalSource CreateSignalSource(ModelPostProcessInfo mppi, IntermediateProductSet intermediateProductSet)
        {
            // TODO: if its just a const zero and one just provide a serializable class ...(it will stick after import right?)
            CommandSignalSource signalProvider;
            try
            {
                signalProvider = this.ApplyInfo.Target.AddComponent<CommandSignalSource>();
            }
            catch (MissingReferenceException mre)
            {
                throw new System.Exception($"Missing Ref exception for: {this.ApplyInfo.Target}. Exception may go away if you repeat the import. (But why is it happening?). {mre.ToString()}");
            }

            // IScalarMechanism
            System.Func<int, string, string, IScalarProvider> GetScalarMechanism = (signalInputType, playableId, objectPath) =>
            {
                switch (signalInputType)
                {
                    case 0: //"Value"
                    default:
                        return null;
                    case 1: // "Playable"
                        var command = CommandFactory.FindCommand(intermediateProductSet, playableId);
                        var provider = command.GetComponent<IScalarProvider>(); // expecting a PlayableClipWrapper 
                        Assert.IsTrue(provider != null, $"null IScalarProvider. for playableId: '{playableId}' ");
                        return provider;
                    case 2: // "Object"
                        var target = MelGameObjectHelper.FindInRoot(this.ApplyInfo.Target.transform, objectPath, mppi.ImportHierarchyLookup);
                        var scalarProvider = target.GetComponent<IScalarProvider>();
                        Assert.IsTrue(scalarProvider is IScalarProvider, $"No IScalar provider was found on target {target?.name}.");
                        return scalarProvider;
                }

            };

            // Enter scalar input
            var enterSignalInputType = this.GetIntWithSuffix("_enter_signal_input_type");
            var enterObjectPath = DuksStrangePathConverter.StrangePathToPath(this.GetStringWithSuffix("_enter_signal_provider"));
            var enterPlayableId = this.GetStringWithSuffix("_enter_signal_playable");
            signalProvider.IEnterScalarLink = (Component)GetScalarMechanism(enterSignalInputType, enterPlayableId, enterObjectPath);

            // Exit
            var exitSignalInputType = this.GetIntWithSuffix("_exit_signal_input_type");
            var exitObjectPath = DuksStrangePathConverter.StrangePathToPath(this.GetStringWithSuffix("_exit_signal_provider"));
            var exitPlayableId = this.GetStringWithSuffix("_exit_signal_playable");
            signalProvider.IExitScalarLink = (Component)GetScalarMechanism(exitSignalInputType, exitPlayableId, exitObjectPath);

            // Enter / exit static values
            signalProvider.EnterSignal = this.GetFloatWithSuffix("_enter_signal");
            signalProvider.ExitSignal = this.GetFloatWithSuffix("_exit_signal");
            return signalProvider;
        }


        void SetupSleepSpreader(ModelPostProcessInfo mppi)
        {
            var spreader = this.ApplyInfo.Target.AddComponent<InteractionSleepSpreader>();
            if (this.GetBoolWithSuffix("_sleep_highlighter_also", true))
            {
                spreader.abstractHighlighter = mppi.AssociateLookup.Get(this.ApplyInfo.Target).Highlighter;
            }
            spreader.ColliderAlso = this.GetBoolWithSuffix("_sleep_collider_also", true);
        }

        void SetupDestroyList()
        {

            if (this.GetIntWithSuffix("_self_destruct_behaviour") == 2) // never destroy
            {
                return;
            }

            var destroyList = this.ApplyInfo.Target.GetOrAddComponent<DestroyList>();

            destroyList.Add(this.ApplyInfo.Target.GetComponent<ClickInteractionHandler>());
            destroyList.Add(this.ApplyInfo.Target.GetComponent<TriggerInteractionHandler>());

            destroyList.Add(this.ApplyInfo.Target.GetComponent<CommandSignalSource>());
            destroyList.Add(this.ApplyInfo.Target.GetComponent<InteractionSleeper>());

            if (this.GetBoolWithSuffix("_destroy_highlighter_also", true))
            {
                destroyList.Add(this.ApplyInfo.Target.GetComponent<AbstractHighlighter>());
            }
            if (this.GetBoolWithSuffix("_destroy_collider_also", true))
            {
                destroyList.Add(this.ApplyInfo.Target.GetComponent<Collider>());
            }
            if (this.GetBoolWithSuffix("_destroy_game_object_also", true))
            {
                destroyList.AddGameObject(this.ApplyInfo.Target);
            }

            var receiver = this.ApplyInfo.Target.GetOrAddComponent<DestroyMessageReceiver>();
            receiver.DestroyList = destroyList;

            if (this.GetIntWithSuffix("_self_destruct_behaviour") == 1)
            { // after first
                var interactionHandler = this.ApplyInfo.Target.GetComponent<ClickInteractionHandler>();
                if (interactionHandler)
                {
                    var listener = this.ApplyInfo.Target.GetOrAddComponent<InteractableDestroyListener>();
                    listener.DestroyList = destroyList;
                    UnityEventTools.AddPersistentListener(interactionHandler.OnInteracted, listener.HandleInteraction);
                }
            }
        }

        public void Link(ModelPostProcessInfo mppi)
        {
            this.SetupDestroyList();
            this.SetupSleepSpreader(mppi);
        }
    }
}

