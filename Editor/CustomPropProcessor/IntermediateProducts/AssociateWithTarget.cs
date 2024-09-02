using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using DuksGames.Argon.Core;

namespace DuksGames.Tools
{
    // some components need to connect to others during import 
    public class AssociationSet
    {
        public AbstractHighlighter Highlighter;
    }

    public class AssociateLookup
    {
        public Dictionary<GameObject, AssociationSet> Associations = new();

        public AssociationSet Get(GameObject target)
        {
            if (this.Associations.TryGetValue(target, out AssociationSet associationSet))
            {
                return associationSet;
            }
            AssociationSet _associationSet = new();
            this.Associations.Add(target, _associationSet);
            return _associationSet;
        }

    }
}