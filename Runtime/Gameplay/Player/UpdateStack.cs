using UnityEngine;
using System.Collections.Generic;
using DuksGames.Argon.Adapters;
using UnityEngine.Assertions;

namespace DuksGames.Argon.Gameplay
{
    /// <summary>
    /// Allows for claims and releases on a shared update callback;
    ///   for update loops that shouldn't be called at the same time as each other. 
    /// </summary>
    public class UpdateStack : MonoBehaviour, IUpdateStack
    {
        Stack<IUpdateLoop> updateLoops = new();

        [SerializeField] GameObject BaseLoopLink;

        public void Start()
        {
            Assert.IsFalse(this.BaseLoopLink.GetComponent<IUpdateLoop>() == null, "need a non null base loop link");
            Assert.IsTrue(this.BaseLoopLink.GetComponent<IUpdateLoop>() is IUpdateLoop, "need a base loop link that implements IUpdateLoop");
            this.updateLoops.Push(this.BaseLoopLink.GetComponent<IUpdateLoop>());
        }

        public bool TakeOver(IUpdateLoop loop)
        {
            if (loop == this.updateLoops.Peek())
            {
                return false;
            }
            this.updateLoops.Push(loop);
            return true;
        }

        public bool Release(IUpdateLoop current)
        {
            if (current == this.updateLoops.Peek())
            {
                this.updateLoops.Pop();
                return true;
            }
            return false;
        }

        void Update()
        {
            this.updateLoops.Peek().DoIUpdateLoop();
        }

    }

}