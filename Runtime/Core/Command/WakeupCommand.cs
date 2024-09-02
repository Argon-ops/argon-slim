using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using DuksGames.Argon.Event;
using System.Linq;
using DuksGames.Argon.Adapters;
using DuksGames.Argon.Utils;

namespace DuksGames.Argon.Core
{



    // Sends a sleep or wake up signal to ISleeps 
    public class WakeupCommand : AbstractCommand
    {
        public Transform[] ISleepComponents;
        List<ISleep> sleeps = new List<ISleep>();

        void Setup()
        {
            this.sleeps.Clear();
            foreach (var t in this.ISleepComponents)
            {
                this.sleeps.AddRange(t.GetComponents<ISleep>());
            }
        }

        void Awake()
        {
            this.Setup();
        }


        protected override Task<CommandResult> _Execute(CommandInfo commandInfo)
        {

            Debug.Log($" will exec WakeUp with {this.sleeps.Count} sleeps");
            foreach (var sleep in this.sleeps)
            {

                // use .Equals to check null becuase sleep == null doesn't work (false when sleep is actually Destroyed). (System.Object.RefEq also doesn't work).
                //   likely this is because references to Destroy()ed components don't truly point to null. because Destroyed components are not actually null.
                //    But, REMIND ME: then why does .Equals work? 
                if (sleep.Equals(null))
                {
                    Debug.Log($"Sleep was null".Pink());
                    continue;
                }

                // Oddly, calling SetIsAwake on pseudo null references does not produce an error
                //   and yet also doesn't seem to *really* call the method. (we don't see any effects of calling that we'd expect) 
                //    so the brow-furrowed null check above is possibly not needed.
                Debug.Log($"Will set iSAwake: {commandInfo.Signal > .5f}".Blue());
                sleep.SetIsAwake(commandInfo.Signal > .5f);
            }

            return Task.FromResult(CommandResult.CompletedResult(commandInfo));
        }
    }
}