using UnityEngine;
using UnityEngine.AI;

namespace TinyShrine.VRoid.Agents.Controllers
{
    /// <summary>
    /// NavMeshAgentを使用してVRoidアバターの自動移動を制御するコンポーネント。
    /// NavMeshに基づく経路探索と移動を行い、アニメーションパラメータを自動更新します。
    /// 目標地点への移動、到達判定、アニメーション同期を統合的に管理します。
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshAgentController : MonoBehaviour
    {
        [Header("Target")]
        [Tooltip("移動目標となるGameObject")]
        [SerializeField]
        private GameObject goal;

        [Tooltip("Start時に自動的に目標地点への移動を開始するかどうか")]
        [SerializeField]
        private bool moveOnStart = true;

        [Header("Arrival")]
        [Tooltip("到達とみなす距離（Stopping Distance より少し大きめ推奨）")]
        [SerializeField]
        private float arriveThreshold = 0.5f;

        [Header("Animator")]
        [Tooltip("VRoidアバターに設定するAnimation Controller")]
        [SerializeField]
        private RuntimeAnimatorController animatorController;

        [Tooltip("Animatorの移動速度パラメータ名（Float型）- BlendTree用の実速度")]
        [SerializeField]
        private string speedParam = "Speed";

        [Tooltip("Animatorのモーション速度パラメータ名（Float型）- 0～1の正規化済み速度")]
        [SerializeField]
        private string motionSpeedParam = "MotionSpeed";

        /// <summary>
        /// NavMeshによる移動制御を行うエージェント。
        /// </summary>
        private NavMeshAgent agent;

        /// <summary>
        /// VRoidアバターのアニメーション制御を行うAnimator。
        /// 子オブジェクトから自動検出されます。
        /// </summary>
        private Animator animator;

        /// <summary>
        /// 目標地点に到達したかどうかのフラグ。
        /// 到達後は移動を停止し、重複判定を防ぎます。
        /// </summary>
        private bool arrived;

        /// <summary>
        /// Speed パラメータの最適化済みハッシュ値。
        /// </summary>
        private int speedHash;

        /// <summary>
        /// MotionSpeed パラメータの最適化済みハッシュ値。
        /// </summary>
        private int motionSpeedHash;

        /// <summary>
        /// 指定された世界座標位置への移動を開始します。
        /// 外部から呼び出し可能で、新しい目標地点を動的に設定できます。
        /// </summary>
        /// <param name="worldPos">移動先の世界座標</param>
        public void MoveTo(Vector3 worldPos)
        {
            // 到達フラグをリセット
            arrived = false;

            // NavMeshAgentが無効化されている場合は移動しない
            if (!agent.enabled)
            {
                return;
            }

            // 移動を再開し、新しい目標地点を設定
            agent.isStopped = false;
            agent.SetDestination(worldPos);
        }

        /// <summary>
        /// 初期化処理。必要なコンポーネントの取得とアニメーションパラメータの設定を行います。
        /// </summary>
        private void Awake()
        {
            // NavMeshAgentコンポーネントを取得（RequireComponentで保証済み）
            agent = GetComponent<NavMeshAgent>();

            // 子オブジェクトからAnimatorを検索（VRoidアバターのAnimatorを想定）
            animator = GetComponentInChildren<Animator>();

            // 指定されたAnimatorControllerを設定
            animator.runtimeAnimatorController = animatorController;

            // NavMeshAgentが位置制御を行うため、Root Motionは無効化
            // これによりアニメーションが移動に干渉しなくなる
            animator.applyRootMotion = false;

            // アニメーションパラメータ名をハッシュ値に変換（パフォーマンス最適化）
            // 文字列比較よりもint比較の方が高速
            speedHash = Animator.StringToHash(speedParam);
            motionSpeedHash = Animator.StringToHash(motionSpeedParam);
        }

        /// <summary>
        /// 開始処理。設定に応じて初期移動を開始します。
        /// </summary>
        private void Start()
        {
            // 自動移動が有効で目標地点が設定されている場合、移動を開始
            if (moveOnStart && goal != null)
            {
                MoveTo(goal.transform.position);
            }
        }

        /// <summary>
        /// 毎フレーム呼び出される更新処理。
        /// NavMeshAgentの移動状態に基づいてアニメーションパラメータを更新し、到達判定を行います。
        /// </summary>
        private void Update()
        {
            // NavMeshAgentの現在速度を計算
            // sqrMagnitudeを使用して平方根計算を回避し、0との比較で最適化
            var speed = agent.velocity.sqrMagnitude > 0 ? agent.velocity.magnitude : 0f;

            // BlendTree用のSpeedパラメータに実際の移動速度を設定
            // アニメーションの歩行/走行ブレンドに使用される
            animator.SetFloat(speedHash, speed);

            // MotionSpeedパラメータに正規化された速度（0～1）を設定
            // agent.speedを最大速度として正規化し、アニメーション速度の調整に使用
            float speed01 = agent.speed > 0f ? Mathf.Clamp01(speed / agent.speed) : 0f;
            animator.SetFloat(motionSpeedHash, speed01);
            // 注意: 常に全力疾走させたい場合は speed01 の代わりに 1f を設定

            // 目標地点への到達判定
            if (!arrived && agent.enabled && !agent.pathPending)
            {
                // pathPendingが false = 経路計算完了
                // remainingDistanceは経路が無い時に Mathf.Infinity になるため hasPath もチェック
                if (agent.hasPath && agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, arriveThreshold))
                {
                    // 到達フラグを立てて移動を停止
                    arrived = true;
                    agent.isStopped = true;
                }
            }
        }
    }
}
