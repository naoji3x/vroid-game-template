using System.Linq;
using UnityEngine;

namespace TinyShrine.Base.Views.Agent
{
    [RequireComponent(typeof(Animator))]
    public class AnimationBridge : MonoBehaviour
    {
        private Animator sourceAnimator;
        private Animator targetAnimator;

        [Header("Animator")]
        [Tooltip("Vroidに設定するAnimation Controller")]
        [SerializeField] private RuntimeAnimatorController animatorController;

        private int speedHash;
        private int motionSpeedHash;
        private int groundedHash;
        private int jumpHash;
        private int freeFallHash;

        void Awake()
        {
            sourceAnimator = GetComponent<Animator>();
            targetAnimator = GetComponentsInChildren<Animator>(includeInactive: true)
                       .FirstOrDefault(a => a.gameObject != this.gameObject);

            if (targetAnimator)
            {
                // AnimatorController を targetAnimator にコピー
                targetAnimator.runtimeAnimatorController = animatorController;
                // Animation Event Receiver を追加
                targetAnimator.gameObject.AddComponent<AnimationEventReceiver>();
            }

            speedHash = Animator.StringToHash("Speed");
            motionSpeedHash = Animator.StringToHash("MotionSpeed");
            groundedHash = Animator.StringToHash("Grounded");
            jumpHash = Animator.StringToHash("Jump");
            freeFallHash = Animator.StringToHash("FreeFall");
        }

        void Update()
        {
            if (!targetAnimator) return;
            targetAnimator.SetFloat(speedHash, sourceAnimator.GetFloat(speedHash));
            targetAnimator.SetFloat(motionSpeedHash, sourceAnimator.GetFloat(motionSpeedHash));
            targetAnimator.SetBool(groundedHash, sourceAnimator.GetBool(groundedHash));
            targetAnimator.SetBool(jumpHash, sourceAnimator.GetBool(jumpHash));
            targetAnimator.SetBool(freeFallHash, sourceAnimator.GetBool(freeFallHash));
        }
    }
}
