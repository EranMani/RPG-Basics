using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplayLogic : MonoBehaviour
    {
        ExperienceLogic experience;

        private void Awake()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<ExperienceLogic>();
        }

        private void Update()
        {
            GetComponent<Text>().text = string.Format("{0:0}", experience.GetPoints());
        }
    }
}
