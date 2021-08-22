using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplayLogic : MonoBehaviour
    {
        BaseStatsLogic baseStats;

        private void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStatsLogic>();
        }

        private void Update()
        {
            GetComponent<Text>().text = string.Format("{0:0}", baseStats.GetLevel());
        }
    }
}
