using UnityEngine;
using UnityEditor;
using DuksGames.Argon.Core;
using System.Collections.Generic;
using System.Linq;
using DuksGames.Argon.Animate;
using UnityEngine.Assertions;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Shared;

namespace DuksGames.Tools
{
    public static class CommandFactory
    {

        // static readonly string _CmdHolderRootName = "zArgonCmdRoot";
        // public static GameObject CreateCmdRoot(GameObject importRoot)
        // {
        //     for (int i = 0; i < importRoot.transform.childCount; ++i) {
        //         if(importRoot.transform.GetChild(i).name == CommandFactory._CmdHolderRootName) {
        //             return importRoot.transform.GetChild(i).gameObject;
        //         }
        //     }

        //     var cmdHolderRoot = EditorUtility.CreateGameObjectWithHideFlags(
        //         CommandFactory._CmdHolderRootName,
        //         HideFlags.None); 
        //     cmdHolderRoot.transform.SetParent(importRoot.transform);
        //     return cmdHolderRoot;
        // }
        // public static Transform GetCmdHolderRoot() 
        // {
        //     // Commands should be owned by separate game objects. 
        //     //  This root object owns those separate game objects.
        //     //   We can only add/edit the import's hierarchy during OnPostProcessModel. So please don't call this earlier
        //     var root = MelGameObjectHelper.FindInScene(CommandFactory._CmdHolderRootName);
        //     Assert.IsFalse(root == null, "Null cmd root. Note that cmd root only exists after OnPostProcessModel is called. "); 
        //     return root.transform;
        // }


        public static PlayableClipWrapper FindMatchingUnderPCWRoot(Transform clipRoot, PlayableClipIngredients pci)
        {
            for (int i = 0; i < clipRoot.childCount; ++i)
            {
                var ch = clipRoot.GetChild(i);
                foreach (var wrapper in ch.GetComponents<PlayableClipWrapper>())
                {
                    if (wrapper)
                    {
                        if (PlayableClipIngredients.IsEquivalent(wrapper.Ingredients, pci))
                        {
                            return wrapper;
                        }
                    }
                }
            }
            return null;
        }

        // public static PlayableClipWrapper FindClipWrapper(Transform owner, PlayableClipIngredients playableClipIngredients)
        // {
        //     // Potentially more than one PClipWrapper attached to a game object
        //     foreach(var playableClipWrapper in owner.GetComponents<PlayableClipWrapper>()) {
        //         // TODO: Keep the RTIngredients but we'll have to deal with this...
        //         if(PlayableClipIngredients.IsEquivalent(playableClipWrapper.Ingredients, playableClipIngredients)){
        //             D.SliLog($"SLI COLLI got a clip wrapper for {owner.name} GRAPH NAME: {playableClipIngredients.GraphName} | {playableClipWrapper.name}");
        //             return playableClipWrapper;
        //         }
        //     }

        //     var rtIngredients = IngredientsSetup.Create(owner.gameObject, playableClipIngredients);
        //     return PlayableClipWrapper.CreateFromIngredients(owner.gameObject, rtIngredients); 
        // }

        // TODO: bug that we don't know how to reproduce: where playable limit events don't fire (PlayCursorAwarePlayable > Notify)
        //   at the end of a playable animation (i.e. door opens). (Rather they are emitted but not received.)
        //     Weirdly uncommenting the above method and commenting the one below, and they re-importing solves this...
        //       But: you can then reverse the commenting/un-commenting again and the problem does not reappear.


        static PlayableClipWrapper CreateFromIngredients(GameObject owner, RTPlayableClipIngredients ingredients)
        {
            var pcw = owner.AddComponent<PlayableClipWrapper>();
            pcw.Ingredients = ingredients;
            return pcw;
        }

        static SignalMessageCommand CreateOverlaySignalCommand(GameObject cmdTarget, float shakeDuration)
        {
            if (shakeDuration <= 0f)
            {
                return cmdTarget.AddComponent<SignalMessageCommand>();
            }

            var cmdOverTime = cmdTarget.AddComponent<SignalLowThenHighCommand>();
            cmdOverTime.Parameters = new OverTimeFunctionParameters
            {
                LeftValue = 1f,
                RightValue = 0f,
                TotalRangeSeconds = shakeDuration
            };
            return cmdOverTime;
        }

        public static PlayableClipWrapper FindClipWrapper(Transform owner, PlayableClipIngredients playableClipIngredients)
        {
            // REMINDER: owner is:
            //    the dedicated object in the case of PlayableCommands.
            //    the Target indicated in the Slider Collider in the case of Slider Collider

            // Search the PCWRoot object for any children with these ingredients
            //   Instead of the search we're doing currently
            var pcwRoot = DuksCommandObjects.GetPCWRoot(owner.root);
            var existingPCW = CommandFactory.FindMatchingUnderPCWRoot(pcwRoot, playableClipIngredients);
            if (existingPCW)
            {
                return existingPCW;
            }

            var rtIngredients = IngredientsSetup.Create(owner.gameObject, playableClipIngredients);

            // Any PlayabeClipWrapper should get its own dedicated object under the zDuks root > PCWRoot
            var pcwHolder = DuksCommandObjects.CreatePCWHolder(owner.transform.root,
                                PlayableClipIngredients.GenerateName(playableClipIngredients));

            return CommandFactory.CreateFromIngredients(pcwHolder, rtIngredients);
        }

        static IEnumerable<Transform> GetTargets(PlayableFabricationInfo fabInfo)
        {

            return fabInfo.WorkTicketData.targets == null ?
                        new List<Transform>() :
                        fabInfo.WorkTicketData.GetTargetNames()
                            .Select((name, idx) =>
                            {
                                // find an object named 'name' in the hierarchy
                                // need importHierarchyLookup wrangling for this for at least one scenario: 
                                //  where an armature is the sole root object. In this case, the armature is renamed (to the file name)
                                //   and it has no mesh. The importHierarchyLookup should find the object (assuming a valid name) and the (!target) condition should never happen afaik.
                                var target = MelGameObjectHelper.FindInRoot(
                                    fabInfo.Owner.transform.root, name, fabInfo.ModelPostProcessInfo.ImportHierarchyLookup); // ) fabInfo.Owner.transform.root.FindRecursive(trans => trans.name == name);

                                if (!target)
                                {
                                    // In at least one case, the FBX exporter renames an object (the root object)--but our target name is based on the
                                    //   blender mesh name. so, if name search fails, search mesh names 
                                    target = fabInfo.Owner.transform.root.FindRecursiveSelfInclusive(trans =>
                                    {
                                        var mf = trans.GetComponent<MeshFilter>();
                                        if (mf)
                                            return mf.sharedMesh.name == name;
                                        return false;
                                    });
                                }
                                Assert.IsFalse(target == null, $"No target named {name} was found. PlayableId: {fabInfo.WorkTicketData.playableId}. PlayableType: {fabInfo.WorkTicketData.playableType} Full WorkTicketData: {fabInfo.WorkTicketData.DumpRecursive()}");
                                return target;
                            });
        }

        public static AbstractCommand CreateCommand(PlayableFabricationInfo fabInfo)
        {
            // types of playable / command
            AbstractCommand command;
            GameObject cmdTarget = fabInfo.Owner.gameObject;

            switch (fabInfo.PlayableType)
            {
                case 0:
                    // event only; fake command
                    command = cmdTarget.AddComponent<FakeCommand>();
                    break;
                case 1:
                case 2:
                default:
                    // animation (1) or looping animation (2)

                    Logger.ImportLog($"FindClipWrapper: {fabInfo.PlayableClipIngredients.Dump()}");

                    var playableCommand = fabInfo.PlayableType == 2 ?
                        cmdTarget.AddComponent<LoopingPlayableCommand>() :
                        cmdTarget.AddComponent<PlayPlayableCommand>();

                    playableCommand.Playable = CommandFactory.FindClipWrapper(fabInfo.Owner, fabInfo.PlayableClipIngredients);

                    playableCommand.AllowsInterrupts = fabInfo.AllowsInterrupts;
                    command = playableCommand;
                    cmdTarget.AddComponent<DDebugClipControls>(); // DEBUG

                    break;
                case 3:
                    // Signal message command
                    // TODO: in general decide whether to allow this function to fail. 
                    //   Big design question: do we let users do stuff that makes no sense and hope they see the warnings.
                    //     Or is it better to fail and raise errors. Which they have to fix or their import stops half way through?
                    //    Already, we have allowed non functioning Playables (in ActionStarterPRocessor...)
                    // enable message
                    var receiverTargets = CommandFactory.GetTargets(fabInfo);
                    Assert.IsFalse(receiverTargets == null, $"rec Targets null. FabInfo Owner: {fabInfo.Owner.name}");

                    // TODO: allow an includeChildren option on the ble side (its already an option actually)
                    //  then, GetInChildren if its opted for.

                    // NOTE: we don't need commands to set initial states for Enableables. It isn't their business
                    //   the Enableables can handle that, if we think that's beneficial

                    List<ISignalHandler> signalReceivers = new();
                    foreach (var rt in receiverTargets)
                    {
                        Assert.IsFalse(rt == null, $"null receiver target. fabInfo Owner {fabInfo.Owner} ");
                        var srs = fabInfo.WorkTicketData.applyToChildren ?
                            rt.GetComponentsInChildren<ISignalHandler>() :
                            rt.GetComponents<ISignalHandler>();
                        Assert.IsFalse(srs.Length == 0, $"Signal receiver target {rt.name} had no ISignalHandler component. Search children {fabInfo.WorkTicketData.applyToChildren}");
                        signalReceivers.AddRange(srs);
                    }

                    SignalMessageCommand signalMessageCommand;

                    if (fabInfo.WorkTicketData.overTime)
                    {
                        var parameters = new OverTimeFunctionParameters
                        {
                            LeftValue = fabInfo.WorkTicketData.lowValue,
                            RightValue = fabInfo.WorkTicketData.highValue,
                            PeriodSeconds = fabInfo.WorkTicketData.periodSeconds,
                            TotalRangeSeconds = fabInfo.WorkTicketData.totalRangeSeconds,
                            TickIntervalSeconds = fabInfo.WorkTicketData.broadcastIntervalSeconds,
                            ClampOutput = fabInfo.WorkTicketData.shouldClampFunction
                        };

                        if (fabInfo.WorkTicketData.overTimeFunction.ToEnum<OverTimeFunctionType>() == OverTimeFunctionType.StartValueEndValue)
                        {
                            var lowThenHigh = fabInfo.WorkTicketData.runIndefinitely ?
                                cmdTarget.AddComponent<SignalLowThenHighIndefiniteCommand>() :
                                cmdTarget.AddComponent<SignalLowThenHighCommand>();
                            lowThenHigh.Parameters = parameters;
                            signalMessageCommand = lowThenHigh;
                        }
                        else
                        {
                            var overTimeCmd = cmdTarget.AddComponent<SignalOverTimeCommand>();
                            overTimeCmd.FunctionIngredients = new OverTimeFunctionIngredients
                            {
                                Parameters = parameters,
                                Type = fabInfo.WorkTicketData.overTimeFunction.ToEnum<OverTimeFunctionType>()
                            };

                            overTimeCmd.OutroSettings = new OutroSettings
                            {
                                OutroBehaviour = fabInfo.WorkTicketData.outroBehaviour.ToEnum<OutroBehaviour>(),
                                Threshold = fabInfo.WorkTicketData.outroThreshold,
                                OutroDestinationValue = fabInfo.WorkTicketData.outroDestinationValue,
                                OutroDestinationValueB = fabInfo.WorkTicketData.outroDestinationValueB,
                                OutroSpeedMultiplier = fabInfo.WorkTicketData.outroSpeedMultiplier,
                            };
                            overTimeCmd.RunIndefinitely = fabInfo.WorkTicketData.runIndefinitely;
                            overTimeCmd.AllowInterrupts = fabInfo.WorkTicketData.allowsInterrupts;
                            overTimeCmd.PickupFromLastState = fabInfo.WorkTicketData.pickUpFromLastState;
                            signalMessageCommand = overTimeCmd;
                        }
                    }
                    else
                    {
                        signalMessageCommand = cmdTarget.AddComponent<SignalMessageCommand>();
                    }
                    signalMessageCommand.IEnableReceiverLinks = signalReceivers.Select(s => (Component)s).ToArray();

                    // TODO: it may be that the object hasn't been set up yet with it IEnableable component yet. (We may have gotten to it first)
                    //   If we really need to warn. Add a wrap-up-time callback (and associated interface) 
                    //     For processors that really need to run checks once everything is ready to be checked.
                    command = signalMessageCommand;
                    break;
                case 4: // shake
                    var shakeCommand = cmdTarget.AddComponent<CameraShakeCommand>();
                    shakeCommand.ShakeConfig = new CameraShakeConfig
                    {
                        ShakeDuration = fabInfo.WorkTicketData.shakeDuration, // TODO: let the python side and WordTicketSide have a child object conforming to 'type' CameraShakeConfig
                        DisplacementDistance = fabInfo.WorkTicketData.shakeDisplacementDistance
                    };
                    command = shakeCommand;
                    break;
                case 5: // screen overlay
                    var sigMessageCommand = CommandFactory.CreateOverlaySignalCommand(cmdTarget,
                                                fabInfo.WorkTicketData.overlayHasDuration ? fabInfo.WorkTicketData.shakeDuration : -1f);

                    var sceneOverlayEnable = cmdTarget.AddComponent<ScreenOverlayEnable>();
                    sceneOverlayEnable.overlayName = fabInfo.WorkTicketData.overlayName;
                    sigMessageCommand.IEnableReceiverLinks = new Component[] { sceneOverlayEnable };
                    var _initialSetter = cmdTarget.AddComponent<InitialEnableStateSetter>();
                    _initialSetter.InitialState = false;
                    _initialSetter.ISignalHandlerLink = sceneOverlayEnable;

                    command = sigMessageCommand;
                    break;
                case 6: // sleep signal
                    var sleepCommand = cmdTarget.AddComponent<WakeupCommand>();
                    sleepCommand.ISleepComponents = CommandFactory.GetTargets(fabInfo).ToArray();
                    command = sleepCommand;
                    break;
                case 7: // headline
                    var headlineCommand = cmdTarget.AddComponent<HeadlineCommand>();
                    headlineCommand.Text = fabInfo.WorkTicketData.headlineText;
                    headlineCommand.Seconds = fabInfo.WorkTicketData.headlineDisplaySeconds;
                    command = headlineCommand;
                    break;
                case 8: // Message bus
                    var messageBusEventCommand = cmdTarget.AddComponent<MessageBusEventCommand>();
                    messageBusEventCommand.MessageBusType = fabInfo.WorkTicketData.messageBusType;
                    messageBusEventCommand.Targets = CommandFactory.GetTargets(fabInfo).Select(t => t.gameObject).ToArray();
                    D.KLog($" Got targets: {messageBusEventCommand.Targets.JoinSelf(g => g.name)}");
                    command = messageBusEventCommand;
                    break;
                case 9: // Destroy signal
                    var destroyCommand = cmdTarget.AddComponent<DestroyMessageCommand>();
                    destroyCommand.IGetDestroyedLinks = CommandFactory.GetTargets(fabInfo)
                                .Select(t => t.gameObject).ToArray();

                    command = destroyCommand;
                    break;
                case 10: // Composite Command
                    var compositeCommand = fabInfo.WorkTicketData.isSequential ?
                            cmdTarget.AddComponent<CompositeSequentialCommand>() : cmdTarget.AddComponent<CompositeCommand>();
                    command = compositeCommand;
                    break;
                case 11: // Wait Seconds
                    var waitSecondsCommand = cmdTarget.AddComponent<WaitSecondsCommand>();
                    waitSecondsCommand.WaitTimeSeconds = fabInfo.WorkTicketData.waitSeconds;
                    command = waitSecondsCommand;
                    break;
                case 12: // Cut Scene

                /// TODO: check why we are getting ftus-water-bloobs as the playable ? not expected right? should have an anim...
                    var cutscene = cmdTarget.AddComponent<CutSceneCommand>();
                    Logger.ImportLog($"FindClipWrapper: {fabInfo.PlayableClipIngredients.Dump()}");
                    cutscene.Playable = CommandFactory.FindClipWrapper(fabInfo.Owner, fabInfo.PlayableClipIngredients);
                    cutscene.CutSceneCamera = cmdTarget.transform.root
                            .FindRecursiveSelfInclusive(tr => tr.name.Equals(fabInfo.WorkTicketData.camera)).GetComponent<Camera>();
                    cutscene.IsCancellable = fabInfo.WorkTicketData.isCancellable;
                    command = cutscene;
                    break;

            }

            if (command is AbstractOvertimeCommand)
            {
                var overtimeCommand = (AbstractOvertimeCommand)command;
                // TODO: why do we not just use WorkTicketData.allowsInterrupts
                overtimeCommand.AllowsInterrupts = fabInfo.AllowsInterrupts;
            }

            CommandFactory.SetupSignalFilters(fabInfo, command);

            command.BehaviourType = fabInfo.WorkTicketData.commandBehaviourType.ToEnum<CommandBehaviourType>();

            command.CustomInfo = fabInfo.WorkTicketData.customInfo;
            return command;
        }

        private static void SetupSignalFilters(PlayableFabricationInfo fabInfo, AbstractCommand command)
        {
            DerivativeProductFactory.ConfigureSignalFilterModifiers(fabInfo, command);
        }

        public static AbstractCommand FindCommand(IntermediateProductSet intermediateProductSet, string playableId)
        {
            Assert.IsFalse(CommandLookup.Instance.Storage == null, "Command Lookup Inst is null?");
            if (CommandLookup.Instance.Storage.ContainsKey(playableId))
            {
                var ccr = CommandLookup.Instance.Storage[playableId];
                return ccr.Command;
            }

            try
            {
                var fabInfo = intermediateProductSet.PlayableFabInfoShare.PlayableIngredients[playableId];
                var command = CommandFactory.CreateCommand(fabInfo);
                CommandLookup.Instance.Storage.Add(playableId, new CommandCreationRecord
                {
                    Command = command,
                    FabricationInfo = fabInfo
                });
                return command;

            }
            catch (KeyNotFoundException)
            {
                Debug.LogWarning($"no playable named ['{playableId}']. Returning null. Contains key ?: {intermediateProductSet.PlayableFabInfoShare.PlayableIngredients.ContainsKey(playableId)}");
                return null;
            }
        }

        static CommandCreationRecord FindExistingCommandRecord(string playableId)
        {
            Assert.IsTrue(CommandLookup.Instance.Storage.ContainsKey(playableId), $"No command with key: '{playableId}' in CommandLookup");
            return CommandLookup.Instance.Storage[playableId];
        }

        // Some commands may need to set up references to each other.
        //  Do that here since this method will be called after all commands have been instantiated
        public static void LinkerPass(IntermediateProductSet intermediateProductSet)
        {
            foreach (var playableId in CommandLookup.Instance.Storage.Keys)
            {
                var ccr = CommandLookup.Instance.Storage[playableId];

                Logger.ImportLog($"{playableId} got ccr? {ccr.FabricationInfo.WorkTicketData.shouldPlayAfter} stor: '{ccr.FabricationInfo.WorkTicketData.playAfterStor}'".Pink());
                Logger.ImportLog($"But playAfter: {ccr.FabricationInfo.WorkTicketData.playAfter}");

                // Not that you asked but: on the python side, playAfterStor is a backing variable for playAfter.
                //   But it doesn't show up in the Command's __annotations__ (TODO: why ) and doesn't have a value here in the import script.
                //    Just pretend its not there. And anyway...
                //     the good news is that playAfter does have a value (when it should)
                //       and we can just use the string stored in it to look up the command we wanted.
                var playAfterName = ccr.FabricationInfo.WorkTicketData.playAfter; // NOT playAfterStor!

                Assert.IsFalse(ccr.FabricationInfo.WorkTicketData.shouldPlayAfter && string.IsNullOrEmpty(playAfterName),
                    $"{playableId} wants to play a command after but doesn't have a playAfterStor string");

                //  play after
                if (ccr.FabricationInfo.WorkTicketData.shouldPlayAfter &&
                    !string.IsNullOrEmpty(playAfterName))
                {
                    try
                    {
                        Logger.ImportLog($"TRY with {playableId} got ccr? ".Pink());

                        var linkedCCR = CommandFactory.FindExistingCommandRecord(playAfterName); //  ccr.FabricationInfo.WorkTicketData.playAfterStor);
                        var link = ccr.FabricationInfo.WorkTicketData.playAfterDeferToLatest ?
                            ccr.Command.gameObject.AddComponent<DeferringCommandChainLink>() :
                            ccr.Command.gameObject.AddComponent<CommandChainLink>();
                        link.Delay = ccr.FabricationInfo.WorkTicketData.playAfterAdditionalDelay;
                        link.NextCommands.Add(linkedCCR.Command);
                        ccr.Command.CommandChainLinks.Add(link);

                        Logger.ImportLog($" NOW: we have {ccr.Command.CommandChainLinks.Count} got ccr?".Green());

                    }
                    catch (System.Exception)
                    {
                        throw new CommandImportException($"Play after linker exception. Command {playableId} trying to link to a command named '{playAfterName}' Command Object: \n", ccr.FabricationInfo.WorkTicketData);
                    }
                }

                // composite
                if (ccr.Command is CompositeCommand compositeCommand)
                {
                    compositeCommand.Commands = ccr.FabricationInfo.WorkTicketData.commandNames
                            .Select(commandName =>
                            {
                                try
                                {
                                    return CommandFactory.FindExistingCommandRecord(commandName).Command;
                                }
                                catch (System.Exception)
                                {
                                    throw new CommandImportException($"Composite Command linker exception. Command {playableId} trying to link " +
                                        $"to a command named '{commandName}'. See below for more info: \n", ccr.FabricationInfo.WorkTicketData);
                                }
                            })
                            .ToArray();

                    // warn
                    foreach (var command in compositeCommand.Commands)
                    {
                        if (command == compositeCommand)
                            Debug.LogWarning($"Composite Command {compositeCommand.name} links to itself. This is guaranteed to infinitely loop. (I hope you know what you're doing )");
                    }
                }
            }
        }
    }
}