using UnityEngine;
using UnityEngine.AI;

namespace TinyShrine.Base.Views.Agent
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshAgentController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField]
        private GameObject goal;

        [SerializeField]
        private bool moveOnStart = true;

        [Header("Arrival")]
        [Tooltip("到達とみなす距離（Stopping Distance より少し大きめ推奨）")]
        [SerializeField]
        private float arriveThreshold = 0.5f;

        [Header("Animator")]
        [Tooltip("Vroidに設定するAnimation Controller")]
        [SerializeField]
        private RuntimeAnimatorController animatorController;

        [Tooltip("Animatorの速度のパラメータ名(Float)")]
        [SerializeField]
        private string speedParam = "Speed";

        [Tooltip("Animatorのモーション速度のパラメータ名(Float)")]
        [SerializeField]
        private string motionSpeedParam = "MotionSpeed";

        private NavMeshAgent agent;
        private Animator animator;
        private bool arrived;

        private int speedHash;
        private int motionSpeedHash;

        /// <summary>指定位置へ移動開始（外部からも呼べます）</summary>
        public void MoveTo(Vector3 worldPos)
        {
            arrived = false;
            if (!agent.enabled)
            {
                return;
            }
            agent.isStopped = false;
            agent.SetDestination(worldPos);
        }

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
            animator.runtimeAnimatorController = animatorController;

            // Animation Event Receiver を追加
            animator.gameObject.AddComponent<AnimationEventReceiver>();

            // エージェントが位置を制御
            animator.applyRootMotion = false;

            speedHash = Animator.StringToHash(speedParam);
            motionSpeedHash = Animator.StringToHash(motionSpeedParam);
        }

        private void Start()
        {
            if (moveOnStart && goal != null)
            {
                MoveTo(goal.transform.position);
            }
        }

        private void Update()
        {
            var speed = agent.velocity.sqrMagnitude > 0 ? agent.velocity.magnitude : 0f;

            // BlendTree用の Speed に実速度を渡す
            animator.SetFloat(speedHash, speed);

            // agent.speed を最大速度として 0..1 に正規化（上限クリップ）
            float speed01 = agent.speed > 0f ? Mathf.Clamp01(speed / agent.speed) : 0f;
            animator.SetFloat(motionSpeedHash, speed01); // 常に全力疾走にしたいなら 1f をセットでもOK

            // 到達判定
            if (!arrived && agent.enabled && !agent.pathPending)
            {
                // remainingDistance は path が無い時に Mathf.Infinity
                if (agent.hasPath && agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, arriveThreshold))
                {
                    arrived = true;
                    agent.isStopped = true;
                }
            }
        }
    }
}
