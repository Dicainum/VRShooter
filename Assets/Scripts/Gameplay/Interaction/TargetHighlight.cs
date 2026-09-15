using UnityEngine;

namespace VRTraining.Gameplay.Interaction
{
    // обводка у объектов, объемный куб у зон
    [RequireComponent(typeof(ScenarioTarget))]
    public sealed class TargetHighlight : MonoBehaviour
    {
        [SerializeField] private GameObject[] _visuals;

        private void Awake()
        {
            SetVisible(false);
        }

        public void SetVisible(bool visible)
        {
            if (_visuals == null)
            {
                return;
            }

            foreach (var visual in _visuals)
            {
                if (visual != null)
                {
                    visual.SetActive(visible);
                }
            }
        }
    }
}
