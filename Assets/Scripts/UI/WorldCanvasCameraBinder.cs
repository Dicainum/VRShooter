using UnityEngine;

namespace VRTraining.UI
{
    [RequireComponent(typeof(Canvas))]
    public sealed class WorldCanvasCameraBinder : MonoBehaviour
    {
        private void Start()
        {
            var canvas = GetComponent<Canvas>();
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                canvas.worldCamera = Camera.main;
            }
        }
    }
}
