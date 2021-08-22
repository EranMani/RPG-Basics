using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawnerLogic : MonoBehaviour
    {
        [SerializeField] DamageTextLogic damageTextPrefab = null;

        public void Spawn(float damageAmount)
        {
            DamageTextLogic instance = Instantiate<DamageTextLogic>(damageTextPrefab, transform);
            instance.SetValue(damageAmount);
        }
    }
}
