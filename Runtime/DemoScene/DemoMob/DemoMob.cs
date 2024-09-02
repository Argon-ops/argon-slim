using UnityEngine;
using System.Collections;
using UnityEngine.AI;

namespace DuksGames.Argon.DemoScene
{

    public class DemoMob : MonoBehaviour
    {
        NavMeshAgent agent;

        [SerializeField, Tooltip("Factor that scales the maximum next-point-search distance")]
        float WanderRange = 20f;

        Animator animator;

        [SerializeField, Tooltip("Disable mob navigation because it forces Argon users to import" +
        " the AI package and makes us have to install a nav mesh asset on their behalf--and how does one " +
        " do that without over writing their existing nav mesh data?")]
        public bool IS_NAVIGATION_DISABLED = true;

        void Start()
        {

            this.agent = this.GetComponent<NavMeshAgent>();
            this.animator = this.GetComponent<Animator>();

            if (this.IS_NAVIGATION_DISABLED) {
                return;
            }
            StartCoroutine(this.Wander());
            StartCoroutine(this.RunAnimator());
        }

        IEnumerator Wander()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(1.8f, 4f));
                Vector3 dest;
                if (this.RandomPoint(this.transform.position, this.WanderRange, out dest))
                {
                    yield return ReachNextDestination(dest);
                    continue;
                }
            }
        }

        IEnumerator RunAnimator()
        {
            while (true)
            {
                this.UpdateAnimator();
                yield return new WaitForSeconds(.3f);
            }
        }

        bool GetReached()
        {
            return this.agent.remainingDistance < this.agent.radius * 2f;
        }

        IEnumerator ReachNextDestination(Vector3 dest)
        {
            this.agent.destination = dest;
            while (true)
            {
                yield return new WaitForEndOfFrame();
                if (this.GetReached())
                {
                    break;
                }
                yield return new WaitForSeconds(3f);
            }
        }

        void UpdateAnimator()
        {
            this.animator.SetBool("ShouldWalk", !this.GetReached());
        }

        bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomPoint = center + Random.insideUnitSphere * range;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, range / 2f, this.agent.areaMask))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }
    }
}