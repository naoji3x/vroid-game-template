using System.Linq;
using UnityEngine;

namespace TinyShrine.Base.Views
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    public class AnimationBridge : MonoBehaviour
    {
        private Animator sourceAnimator;
        private Animator targetAnimator;

        void Awake()
        {
            sourceAnimator = GetComponent<Animator>();
            targetAnimator = GetComponentsInChildren<Animator>(includeInactive: true)
                       .FirstOrDefault(a => a.gameObject != this.gameObject);
        }

        // デバッグ用にAnimatorの値を画面上に表示
        void OnGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 20;
            style.normal.textColor = Color.white;

            if (sourceAnimator != null)
            {
                var speed = sourceAnimator.GetFloat("Speed");
                var motionSpeed = sourceAnimator.GetFloat("MotionSpeed");
                var grounded = sourceAnimator.GetBool("Grounded");
                var jump = sourceAnimator.GetBool("Jump");
                var freeFall = sourceAnimator.GetBool("FreeFall");

                GUI.Label(new Rect(10, 10, 800, 30), $"SourceAnimator: Speed={speed}, MotionSpeed={motionSpeed}, Grounded={grounded}, Jump={jump}, FreeFall={freeFall}", style);
            }
            if (targetAnimator != null)
            {
                var speed = targetAnimator.GetFloat("Speed");
                var motionSpeed = targetAnimator.GetFloat("MotionSpeed");
                var grounded = targetAnimator.GetBool("Grounded");
                var jump = targetAnimator.GetBool("Jump");
                var freeFall = targetAnimator.GetBool("FreeFall");

                GUI.Label(new Rect(10, 40, 800, 30), $"TargetAnimator: Speed={speed}, MotionSpeed={motionSpeed}, Grounded={grounded}, Jump={jump}, FreeFall={freeFall}", style);
            }
        }

        void Update()
        {
            if (!targetAnimator) return;

            targetAnimator.SetFloat("Speed", sourceAnimator.GetFloat("Speed"));
            targetAnimator.SetFloat("MotionSpeed", sourceAnimator.GetFloat("MotionSpeed"));
            targetAnimator.SetBool("Grounded", sourceAnimator.GetBool("Grounded"));
            targetAnimator.SetBool("Jump", sourceAnimator.GetBool("Jump"));
            targetAnimator.SetBool("FreeFall", sourceAnimator.GetBool("FreeFall"));
        }
    }
}
