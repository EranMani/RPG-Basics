using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HealthDisplayLogic will update the health points text component of the player
/// </summary>
namespace RPG.Attributes
{
    public class HealthDisplayLogic : MonoBehaviour
    {
        HealthLogic health;

        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<HealthLogic>();
        }

        private void Update()
        {
            GetComponent<Text>().text = string.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
        }
    }
}
