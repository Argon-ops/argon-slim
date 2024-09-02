using UnityEngine;
using System.Collections.Generic;
using DuksGames.Argon.Core;

namespace DuksGames.Tools
{

    public class CommandCreationRecord
    {
        public AbstractCommand Command;
        public PlayableFabricationInfo FabricationInfo;
    }

    public class CommandLookup
    {
        static CommandLookup _Instance;

        public static CommandLookup Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new CommandLookup();
                }
                return _Instance;
            }
        }

        public Dictionary<string, CommandCreationRecord> Storage { get; private set; } = new Dictionary<string, CommandCreationRecord>();

        public void Clear()
        {
            this.Storage.Clear();
        }

    }
}