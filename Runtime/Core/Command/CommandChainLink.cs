using UnityEngine;
using System.Collections.Generic;
using DuksGames.Argon.Event;
using System.Collections;
using DuksGames.Argon.Utils;

namespace DuksGames.Argon.Core
{
    public class CommandChainLink : MonoBehaviour
    {
        public List<AbstractCommand> NextCommands = new List<AbstractCommand>();
        public float Delay;

        public void HandleCommandEnded(CommandEvent ce)
        {
            if (!this.enabled)
            {
                return;
            }
            if (ce.Type == CommandEventType.WillStart)
            {
                return;
            }
            StartCoroutine(WaitForDelay(ce));
        }

        protected virtual IEnumerator WaitForDelay(CommandEvent ce)
        {
            yield return new WaitForSeconds(this.Delay);

            foreach (var cmd in this.NextCommands)
            {
                cmd.Execute(ce.Result.CommandInfo);
            }
        }
    }

}