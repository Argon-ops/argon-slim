using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using DuksGames.Argon.Event;
using System.Linq;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Utils;
using UnityEngine.Assertions;

namespace DuksGames.Argon.Core
{

    public class DestroyMessageCommand : AbstractCommand
    {

        public GameObject[] IGetDestroyedLinks;


        protected override Task<CommandResult> _Execute(CommandInfo commandInfo)
        {
            // custom destroy
            foreach (var component in this.IGetDestroyedLinks)
            {
                foreach (var destroyed in component.GetComponents<ICustomDestroyMessageReceiver>())
                {
                    destroyed.PreCustomDestroyMessage();
                }
            }
            foreach (var component in this.IGetDestroyedLinks)
            {
                foreach (var d in component.GetComponents<ICustomDestroyMessageReceiver>())
                {
                    d.CustomDestroy();
                }
            }

            // simple destroy
            foreach (var component in this.IGetDestroyedLinks)
            {
                foreach (var d in component.GetComponents<ISimpleDestroyReceiver>())
                {
                    GameObject.Destroy((Component)d);
                }
            }

            return Task.FromResult(CommandResult.CompletedResult(commandInfo));
        }

    }
}