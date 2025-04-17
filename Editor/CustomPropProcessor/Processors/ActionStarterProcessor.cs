using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DuksGames.Argon.Animate;
using UnityEngine.Assertions;
using System;
using DuksGames.Argon.Shared;

namespace DuksGames.Tools
{

    public class ActionStarterKeySet : AbstractCustomPropKeySet<ActionStarterProcessor>
    {
        public override string TargetKey => "mel_action_starter";

        public override IEnumerable<string> GetKeys()
        {
            yield return null;
        }

        public override bool ContainsKey(string key)
        {
            return key.StartsWith("mel_AS_");
        }


    }


    public class ActionStarterProcessor : AbstractCustomPropProcessor,
            IApplyCustomProperties,
            IIntermediateProductProducer
    {

        HashSet<StarterWorkTicketData> _workTickets = new HashSet<StarterWorkTicketData>();

        void MakeWorkTickets()
        {
            foreach (KeyValuePair<string, dynamic> pair in this.Config.lookup)
            {
                if (pair.Key == this.KeySet.TargetKey)
                {
                    continue;
                }

                var ticketData = new StarterWorkTicketData();
                JsonUtility.FromJsonOverwrite(pair.Value, ticketData);
                this._workTickets.Add(ticketData);
            }
        }

        public void Apply()
        {
            this.MakeWorkTickets();
        }


        Transform FindOrCreateOwner(ModelPostProcessInfo mppi, StarterWorkTicketData ticketData)
        {
            // Playables need an 'owner'; a game object to attach their components to.
            //  all cmds get their own owner objects

            Assert.IsFalse(string.IsNullOrEmpty(ticketData.playableId), $"Need a non null playable id. {this.ApplyInfo.Target.name}");
            return DuksCommandObjects.FindOrCreateCommandHolder(this.ApplyInfo.Target.transform.root, ticketData.playableId).transform;
        }

        class ClipFilterResult
        {
            public AnimationClip clip;
            public string targetName;
        }

        IEnumerable<ClipFilterResult> FilterClips(StarterWorkTicketData ticketData, Dictionary<string, AnimationClip> clips)
        {

            if (!ticketData.IsAnimationClipRequired())
            {
                return new ClipFilterResult[] { };
            }

            var targetNames = new HashSet<string>();
            // enforce unique names
            targetNames.UnionWith(ticketData.GetTargetNames());

            if (clips.Count == 0)
            {
                return new ClipFilterResult[] { };
            }

            return targetNames.Select(tn =>
            {
                try
                {
                    return new ClipFilterResult
                    {
                        targetName = tn,
                        clip = clips.Values.First(clip => clip.name == $"{tn}|{ticketData.animAction}")
                    };
                }
                catch (System.ArgumentNullException ne)
                {
                    Debug.LogWarning($"ARGON: no animation clip for target. PlayableId: {ticketData.playableId}. PlayableType: {ticketData.playableType}. Target '{tn}' and blender action '{ticketData.animAction}'. Looking for a clip named: '{tn}|{ticketData.animAction}'. You may have chosen a target that wasn't associated with the action. For example, an object that is controlled by an armature instead of the armature itself? ");
                    throw ne;
                }
                catch (System.InvalidOperationException)
                {
                    Debug.LogWarning($"ARGON: for command {ticketData.playableId} :: No animation clip for target '{tn}' and blender action '{ticketData.animAction}'. Looking for a clip named: '{tn}|{ticketData.animAction}'. You may have chosen a target that wasn't associated with the action. Perhaps (for example, just a guess) an object that is controlled by an armature instead of the armature itself? ".Pink());
                    return null;
                }
            });
        }

        PlayableClipIngredients CreateClipIngredients(ModelPostProcessInfo mppi, StarterWorkTicketData ticketData, Dictionary<string, AnimationClip> clips)
        {
            var filterClipResults = this.FilterClips(ticketData, clips).Where(result => result != null);
            Logger.Log($"Create Clip Ingredients: {filterClipResults.JoinSelf(fcr => fcr.targetName)}".Orange());
            var audioClip = MelGameObjectHelper.FindInProject<AudioClip>(ticketData.audioClipName);

            var audioSourceOwner = GameObject.Find(ticketData.audioObjectPath);
            if (audioSourceOwner == null) audioSourceOwner = mppi.Root;
            var audioSource = MelGameObjectHelper.AddIfNotPresent<AudioSource>(audioSourceOwner);

            var pci = new PlayableClipIngredients()
            {
                AnimationClips = filterClipResults.Select(result => result.clip).ToArray(),
                AnimationTargets = filterClipResults.Select(result => mppi.Root.transform.FindRecursive(result.targetName).gameObject).ToArray(),
                AudioClip = audioClip,
                AudioSource = audioSource,
                ShouldLoopAudio = ticketData.loopAudio,
                AudioAlwaysForwards = ticketData.audioAlwaysForwards,
                GraphName = $"PCI_{ticketData.playableId}",
            };
            return pci;
        }


        PlayableFabricationInfo CreateFabricationInfo(ModelPostProcessInfo mppi, StarterWorkTicketData ticketData, Dictionary<string, AnimationClip> clips)
        {
            var owner = this.FindOrCreateOwner(mppi, ticketData);
            var pci = this.CreateClipIngredients(mppi, ticketData, clips);


            Assert.IsFalse(owner == null, $"null playable. mppi.root: {mppi.Root} \n cmd behaviour type : {ticketData.commandBehaviourType} ");

            return new PlayableFabricationInfo
            {
                PlayableClipIngredients = pci,
                PlayableType = ticketData.playableType,
                AllowsInterrupts = ticketData.allowsInterrupts,
                WorkTicketData = ticketData,
                Owner = owner,
                ModelPostProcessInfo = mppi,
            };
        }

        public void SetProductSet(ModelPostProcessInfo mppi, IntermediateProductSet intermediateProductSet)
        {
            foreach (var ticket in this._workTickets)
            {
                if (string.IsNullOrEmpty(ticket.playableId))
                {
                    continue;
                }
                if (intermediateProductSet.PlayableFabInfoShare.PlayableIngredients.ContainsKey(ticket.playableId))
                {
                    // ...
                }
                var ingredients = this.CreateFabricationInfo(mppi, ticket, intermediateProductSet.Clips);
                intermediateProductSet.PlayableFabInfoShare.PlayableIngredients.Add(ticket.playableId, ingredients);
            }

            this.MakeCommands(mppi, intermediateProductSet);
            this._workTickets.Clear();
        }

        void MakeCommands(ModelPostProcessInfo mppi, IntermediateProductSet intermediateProductSet)
        {
            foreach (var playableId in intermediateProductSet.PlayableFabInfoShare.PlayableIngredients.Keys)
            {
                CommandFactory.FindCommand(intermediateProductSet, playableId);
            }
        }
    }


}
