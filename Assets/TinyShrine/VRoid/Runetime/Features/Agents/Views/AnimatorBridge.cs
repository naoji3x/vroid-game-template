using System.Linq;
using UnityEngine;

namespace TinyShrine.VRoid.Agents.Views
{
    /// <summary>
    /// ソースAnimatorからターゲットAnimatorへアニメーションパラメータを同期するブリッジクラス。
    /// VRoidアバターのアニメーション制御において、親オブジェクトのAnimatorから
    /// 子オブジェクト（VRoidアバター）のAnimatorにパラメータをリアルタイムで転送します。
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimatorBridge : MonoBehaviour
    {
        /// <summary>
        /// アニメーションパラメータの送信元Animator（このGameObjectに付与）。
        /// </summary>
        private Animator sourceAnimator;

        /// <summary>
        /// アニメーションパラメータの送信先Animator（子オブジェクトのVRoidアバター）。
        /// </summary>
        private Animator targetAnimator;

        [Header("Animator Settings")]
        [Tooltip("VRoidアバターに設定するAnimation Controller")]
        [SerializeField]
        private RuntimeAnimatorController? animatorController;

        // アニメーションパラメータのハッシュ値（パフォーマンス最適化のため事前計算）

        /// <summary>移動速度パラメータのハッシュ値。</summary>
        private int speedHash;

        /// <summary>モーション速度パラメータのハッシュ値。</summary>
        private int motionSpeedHash;

        /// <summary>接地状態パラメータのハッシュ値。</summary>
        private int groundedHash;

        /// <summary>ジャンプ状態パラメータのハッシュ値。</summary>
        private int jumpHash;

        /// <summary>自由落下状態パラメータのハッシュ値。</summary>
        private int freeFallHash;

        /// <summary>
        /// 初期化処理。ソースとターゲットのAnimatorを取得し、
        /// アニメーションパラメータのハッシュ値を事前計算します。
        /// </summary>
        private void Awake()
        {
            // このGameObjectに付属するAnimatorをソースとして取得
            sourceAnimator = GetComponent<Animator>();

            // 子オブジェクト内で、このGameObject以外に付属するAnimatorをターゲットとして取得
            // （VRoidアバターのAnimatorを想定）
            targetAnimator = GetComponentsInChildren<Animator>(includeInactive: true)
                .FirstOrDefault(a => a.gameObject != gameObject);

            // 指定されたAnimatorControllerをターゲットAnimatorに設定
            if (targetAnimator)
            {
                targetAnimator.runtimeAnimatorController = animatorController;
            }

            // アニメーションパラメータ名をハッシュ値に変換（パフォーマンス最適化）
            // 文字列比較よりもint比較の方が高速
            speedHash = Animator.StringToHash("Speed");
            motionSpeedHash = Animator.StringToHash("MotionSpeed");
            groundedHash = Animator.StringToHash("Grounded");
            jumpHash = Animator.StringToHash("Jump");
            freeFallHash = Animator.StringToHash("FreeFall");
        }

        private void Update()
        {
            if (!targetAnimator)
            {
                return;
            }

            targetAnimator.SetFloat(speedHash, sourceAnimator.GetFloat(speedHash));
            targetAnimator.SetFloat(motionSpeedHash, sourceAnimator.GetFloat(motionSpeedHash));
            targetAnimator.SetBool(groundedHash, sourceAnimator.GetBool(groundedHash));
            targetAnimator.SetBool(jumpHash, sourceAnimator.GetBool(jumpHash));
            targetAnimator.SetBool(freeFallHash, sourceAnimator.GetBool(freeFallHash));
        }
    }
}
