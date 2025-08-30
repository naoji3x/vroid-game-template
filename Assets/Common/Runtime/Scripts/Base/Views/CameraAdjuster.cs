namespace TinyShrine.Base.Views
{
    using UnityEngine;

    /// <summary>
    /// カメラのアスペクト比を調整するコンポーネント。
    /// 横幅を10unitsに固定し、画面のアスペクト比に基づいてカメラのorthographicSizeを設定します.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraAdjuster : MonoBehaviour
    {
        [SerializeField]
        private float targetWidth = 10f; // 横幅を10unitsに固定

        private void Start()
        {
            var cam = this.GetComponent<Camera>();
            var screenAspect = (float)Screen.width / Screen.height;
            cam.orthographicSize = this.targetWidth / (2f * screenAspect);
        }
    }
}
