// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using DuksGames.Argon.Core;
// using DuksGames.Argon.Event;
// using DuksGames.Argon.Shared;
// using UnityEngine;

// namespace DuksGames.Argon.Core
// {
//     public class SleeperCommandsListener : MonoBehaviour
//     {
//         public AbstractCommand[] Commands;

//         public AbstractHighlighter[] Highlighters;

//         void Start()
//         {
//             // COMPLAINT: this class has problems: there is no
//             //   mechanism for deciding how to behave when there are multiple commands
//             //    presumably we should wait for all to start and all to end?
//             //     but, of course, there's no guarantee that they'll all start at the same time or ever
//             //    ... maybe rethink our whole approach even though this works for narrow use case?
//             foreach(var command in this.Commands) {

//                 command.GetCommandEvent().AddListener(this.HandleCommandEvent);
//             }
//         }

//         private async void HandleCommandEvent(CommandEvent commandEvent)
//         {
//             await Task.Delay(1); // Cheesy work around: TODO: let listeners see the task

//             foreach (var hl in this.Highlighters)
//             {
//                 hl.SetIsAwake(commandEvent.Type != CommandEventType.WillStart);
//                 hl.SetState(commandEvent.Type != CommandEventType.WillStart ? Adapters.EClickBeaconState.Visible : Adapters.EClickBeaconState.Off);
//             }

//         }
//     }
// }
