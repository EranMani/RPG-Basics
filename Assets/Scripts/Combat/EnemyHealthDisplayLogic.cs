using RPG.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplayLogic : MonoBehaviour
    {
        FighterLogic fighter;

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<FighterLogic>();
        }

        private void Update()
        {
            if (fighter.GetTarget() == null)
            {
                GetComponent<Text>().text = "N/A";
                return;
            }
            HealthLogic health = fighter.GetTarget();
            GetComponent<Text>().text = string.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
        }
    }
}
