using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace DuksGames.Tools
{
    [RequireComponent(typeof(Animator))]
    public class DAnimEventReceiver : MonoBehaviour
    {
        public AnimationClip clip;
        PlayableGraph graph;
        public void OnFakeAnimEvent(string param)
        {
            Debug.Log($"on fake event with {param}");
        }

        void Start()
        {
            // set up a playable: low econ way of playing the clip.
            this.graph = PlayableGraph.Create();
            this.graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            var playableOutput = AnimationPlayableOutput.Create(this.graph, "Animation", GetComponent<Animator>());
            var clipPlayable = AnimationClipPlayable.Create(this.graph, this.clip);
            playableOutput.SetSourcePlayable(clipPlayable);

            // force animations to start so we can see the events...
            this.graph.Play();
        }

        void OnDisable()
        {
            this.graph.Destroy();
        }
    }
}