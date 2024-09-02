using UnityEngine;
using System;
using DuksGames.Argon.Event;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DuksGames.Argon.Utils;

namespace DuksGames.Argon.Gameplay
{
    public class CommandForwarder : MonoBehaviour
    {

        Dictionary<string, IExecute> _commands = new();

        void Start()
        {
            MessageBus.Instance.OnBroadcast.RemoveListener(this.HandleMessageBus);
            MessageBus.Instance.OnBroadcast.AddListener(this.HandleMessageBus);
            this.RefreshCommands();
        }

        void RefreshCommands()
        {
            foreach(var root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                foreach(var exe in root.GetComponentsInChildren<IExecute>())
                {
                    var key = $"{root.name}.{((Component)exe).name}";
                    if(this._commands.ContainsKey(key))
                    {
                        Debug.LogWarning($"Command Forwarder found two commands with the same root-name.name : {key}. Only the first one will be added.");
                        continue;
                    }
                    this._commands.Add(key, exe);
                }
            }
        }

        IExecute FindCommand(string name)
        {
            if(this._commands.ContainsKey(name))
            {
                return this._commands[name];
            }

            foreach(var key in this._commands.Keys)
            {
                if(key.EndsWith(name))
                {
                    Debug.Log($"Did find key: {key}");
                    return this._commands[key];
                }
            }
            return null;
        }

        private void HandleMessageBus(MessageBusEvent msgBusEvent)
        {
            Debug.Log($"Cmd Forwarder got: {msgBusEvent.Type}");
            if(msgBusEvent.Type != "IExecuteForwarder")
            {
                return;
            }
            Debug.Log($"Cmd Forwarder got: {msgBusEvent.Type}".Pink());

            var exe = this.FindCommand(msgBusEvent.CustomInfo);
            if(exe == null)
            {
                Debug.Log($"no IExecute with name: {msgBusEvent.CustomInfo}");
                return;
            }

            exe.Execute(msgBusEvent.CommandInfo);
        }
    }
}