using UnityEngine;
using UnityEngine.Events;

namespace TinyShrine.VRoid.Agents.Views
{
    /// <summary>
    /// アニメーションイベントを受信してUnityEventで処理を配信するコンポーネント。
    /// Animatorコンポーネントと同じGameObjectに配置され、アニメーションから送信される
    /// AnimationEventをUnityEventとして外部に通知します。
    /// 音声再生やその他の処理は、外部のコンポーネントがUnityEventを購読することで実現します。
    /// </summary>
    public class AnimationEventReceiver : MonoBehaviour
    {
        /// <summary>
        /// Gets 足音アニメーションイベント発生時に呼び出されるUnityEvent。
        /// 外部コンポーネントがこのイベントを購読して音声再生等の処理を実行できます。
        /// </summary>
        public UnityEvent<AnimationEvent> OnFootstepEvent { get; } = new();

        /// <summary>
        /// Gets 着地アニメーションイベント発生時に呼び出されるUnityEvent。
        /// 外部コンポーネントがこのイベントを購読して音声再生等の処理を実行できます。
        /// </summary>
        public UnityEvent<AnimationEvent> OnLandEvent { get; } = new();

        /// <summary>
        /// 足音アニメーションイベントの受信メソッド。
        /// アニメーション内のAnimationEventから呼び出され、対応するUnityEventを発火します。
        /// </summary>
        /// <param name="evt">アニメーションイベントの詳細情報</param>
        public void OnFootstep(AnimationEvent evt) => OnFootstepEvent.Invoke(evt); // UnityEventを発火

        /// <summary>
        /// 着地アニメーションイベントの受信メソッド。
        /// アニメーション内のAnimationEventから呼び出され、対応するUnityEventを発火します。
        /// </summary>
        /// <param name="evt">アニメーションイベントの詳細情報</param>
        public void OnLand(AnimationEvent evt) => OnLandEvent.Invoke(evt); // UnityEventを発火

        /// <summary>
        /// コンポーネント破棄時のクリーンアップ処理。
        /// メモリリークを防ぐため、登録されたすべてのイベントリスナーを削除します。
        /// </summary>
        private void OnDestroy()
        {
            // イベントリスナーを削除してメモリリークを防止
            OnFootstepEvent.RemoveAllListeners();
            OnLandEvent.RemoveAllListeners();
        }
    }
}
