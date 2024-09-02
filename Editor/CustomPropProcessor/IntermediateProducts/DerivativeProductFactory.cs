using UnityEngine;
using UnityEditor;
using DuksGames.LogicBlocks;
using DuksGames.Argon.Core;
using System;
using DuksGames.Argon.Shared;

namespace DuksGames.Tools
{

    public static class DerivativeProductFactory
    {

        static ToggleModifier CreateToggleModifier(AbstractCommand command)
        {
            var toggleModifier = MelGameObjectHelper.AddIfNotPresent<ToggleModifier>(command.gameObject);
            var thresholdProvider = MelGameObjectHelper.AddIfNotPresent<ThresholdOnOffProvider>(command.gameObject);

            toggleModifier.IOnOffStateProviderLink = thresholdProvider;
            thresholdProvider.IProgressLinkObject = command;

            return toggleModifier;
        }

        public static void ConfigureCommandModifiers(CommandBehaviourType interpretStyle, AbstractCommand command)
        {
            switch (interpretStyle)
            {
                case CommandBehaviourType.ToggleAndRestart:
                default:

                    var mod = DerivativeProductFactory.CreateToggleModifier(command);
                    command.CommandModifiers.Add(mod);
                    break;

                case CommandBehaviourType.FlipDirections:
                    break;
                case CommandBehaviourType.RestartForwards:
                    break;
            }

        }

        public static void ConfigureSignalFilterModifiers(PlayableFabricationInfo fabInfo, AbstractCommand command)
        {
            switch (fabInfo.WorkTicketData.signalFilters.ToEnum<SignalFilterModifierType>())
            {
                case SignalFilterModifierType.DontFilter:
                default:
                    break;
                case SignalFilterModifierType.ConstantValue:
                    var cvm = MelGameObjectHelper.AddIfNotPresent<ConstantValueModifier>(fabInfo.Owner.gameObject);
                    cvm.ConstantValue = fabInfo.WorkTicketData.signalConstantValue;
                    command.CommandModifiers.Add(cvm);
                    break;
                case SignalFilterModifierType.OneMinusSignal:
                    throw new System.NotImplementedException("TODO: OneMinusSignal modifier");
            }
        }



    }
}