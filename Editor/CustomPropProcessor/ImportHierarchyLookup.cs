using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace DuksGames.Tools
{
    // Maintain a map: original-name-in-fbx => instanceId.
    //  Because names of objects in the fbx hierarchy can change 
    //    when imported to Unity. 
    //   
    public class ImportHierarchyLookup
    {
        Dictionary<string, int> _lookup = new();
        Dictionary<int, string> _importNameLookup = new();

        public void Add(GameObject go)
        {
            this._lookup.Add(go.name, go.GetInstanceID());
            this._importNameLookup.Add(go.GetInstanceID(), go.name);
        }

        public bool TryGetInstanceId(string importName, out int instanceId)
        {
            return this._lookup.TryGetValue(importName, out instanceId);
        }

        public bool TryGetImportName(int instanceId, out string importName)
        {
            return this._importNameLookup.TryGetValue(instanceId, out importName);
        }

        public void Clear()
        {
            this._lookup.Clear();
            this._importNameLookup.Clear();
        }

        public string DDump()
        {
            return string.Join(',', this._lookup.Select(m => $"{m.Key} : {m.Value}"));
        }
    }


}