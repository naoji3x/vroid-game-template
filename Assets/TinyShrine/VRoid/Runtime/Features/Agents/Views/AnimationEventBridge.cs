using System.Linq;
using UnityEngine;

namespace TinyShrine.VRoid.Agents.Views
{
    /// <summary>
    /// VRoidアバターのアニメーションイベントをブリッジして音声効果を再生するコンポーネント。
    /// 子オブジェクトのAnimatorからAnimationEventReceiverを自動検出・追加し、
    /// アニメーションイベントの受信時に対応する音声効果を再生します。
    /// このコンポーネントは親オブジェクトに配置し、VRoidアバターは子オブジェクトに配置する構成で使用します。
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AnimationEventBridge : MonoBehaviour
    {
        /// <summary>
        /// アニメーションイベントの受信対象となるターゲットAnimator。
        /// 子オブジェクト内で最初に見つかったAnimatorが自動設定されます。
        /// </summary>
        private Animator targetAnimator;

        [Header("Sound Effects")]
        [Tooltip("足音のAudioClip")]
        [SerializeField]
        private AudioClip? footstepSound;

        [Tooltip("着地音のAudioClip")]
        [SerializeField]
        private AudioClip? landSound;

        [Header("Audio Settings")]
        [Tooltip("足音の音量（0.0〜1.0）")]
        [Range(0f, 1f)]
        [SerializeField]
        private float footstepVolume = 0.7f;

        [Tooltip("着地音の音量（0.0〜1.0）")]
        [Range(0f, 1f)]
        [SerializeField]
        private float landVolume = 0.8f;

        /// <summary>
        /// 音声再生に使用するAudioSourceコンポーネント。
        /// RequireComponentにより自動で要求され、Awakeで取得されます。
        /// </summary>
        private AudioSource audioSource;

        /// <summary>
        /// 初期化処理。AudioSourceとターゲットAnimatorを取得し、
        /// AnimationEventReceiverを設定してイベントリスナーを登録します。
        /// </summary>
        private void Awake()
        {
            // AudioSourceコンポーネントを取得（RequireComponentで保証）
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError($"AudioSourceコンポーネントが見つかりません: {gameObject.name}", this);
            }

            // 子オブジェクト内でこのGameObject以外の最初のAnimatorを検索
            // VRoidアバターのAnimatorを想定
            targetAnimator = GetComponentsInChildren<Animator>(includeInactive: true)
                .FirstOrDefault(a => a.gameObject != gameObject);

            if (targetAnimator)
            {
                // AnimationEventReceiverが存在しない場合は自動追加
                if (!targetAnimator.gameObject.TryGetComponent<AnimationEventReceiver>(out var receiver))
                {
                    receiver = targetAnimator.gameObject.AddComponent<AnimationEventReceiver>();
                }

                // イベントリスナーを登録して、アニメーションイベントを受信
                receiver.OnFootstepEvent.AddListener(OnFootstep);
                receiver.OnLandEvent.AddListener(OnLand);
            }
        }

        /// <summary>
        /// 足音アニメーションイベントのハンドラー。
        /// AnimationEventReceiverから呼び出され、足音AudioClipを再生します。
        /// </summary>
        /// <param name="evt">アニメーションイベントの詳細情報</param>
        private void OnFootstep(AnimationEvent evt)
        {
            // 足音AudioClipが設定されている場合のみ再生
            if (footstepSound != null)
            {
                audioSource.PlayOneShot(footstepSound, footstepVolume);
            }
        }

        /// <summary>
        /// 着地アニメーションイベントのハンドラー。
        /// AnimationEventReceiverから呼び出され、着地音AudioClipを再生します。
        /// </summary>
        /// <param name="evt">アニメーションイベントの詳細情報</param>
        private void OnLand(AnimationEvent evt)
        {
            // 着地音AudioClipが設定されている場合のみ再生
            if (landSound != null)
            {
                audioSource.PlayOneShot(landSound, landVolume);
            }
        }
    }
}
