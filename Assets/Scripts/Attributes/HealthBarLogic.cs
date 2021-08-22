using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HealthBarLogic will choose when to show the health bar, and will update the bar in scale mode
/// </summary>
namespace RPG.Attributes
{
    public class HealthBarLogic : MonoBehaviour
    {
        [SerializeField] HealthLogic healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas rootCanvas = null;

        void Update()
        {
            // Check if health value is at the maximum or minimum limits
            if (Mathf.Approximately(healthComponent.GetFraction(), 0)
             || Mathf.Approximately(healthComponent.GetFraction(), 1))
            {
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;
            foreground.localScale = new Vector3(healthComponent.GetFraction(), 1, 1);
        }
    }
}
