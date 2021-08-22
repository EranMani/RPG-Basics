using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using System;
using GameDevTV.Utils;
using UnityEngine.Events;

/// <summary>
/// HealthLogic will determine holder health values by taking damage and dying
/// This goes both for the player and the enemies
/// </summary>

namespace RPG.Attributes
{
    public class HealthLogic : MonoBehaviour, ISaveable
    {
        [SerializeField] float regenerationPercentage = 70;
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {

        }
     
        LazyValue<float> m_health;
        bool m_isDead = false;

        Animator m_anim;
        
        private void Awake()
        {
            m_anim = GetComponent<Animator>();

            // Holder initial health will be derived from the STAT script
            m_health = new LazyValue<float>(GetInitialHealth);
        }

        private void Start()
        {
            m_health.ForceInit();
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStatsLogic>().GetStat(Stat.Health);
        }

        public void TakeDamage(GameObject instigator, float damage)
        {       

            // Pick the highest value in order to clamp the health value not going to negative value
            m_health.value = Mathf.Max(m_health.value - damage, 0);

            if (m_health.value == 0 && !m_isDead)
            {
                // When dying, call the die event, update the animation and let the player gain XP 
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
            else
            {
                // When taking damage, play a sound and spawn the damage hit text ui
                takeDamage.Invoke(damage);
            }
        }

        private void Die()
        {
            if (m_isDead) return;

            m_isDead = true;
            m_anim.SetTrigger("die");
            GetComponent<ActionSchedularLogic>().CancelCurrentAction();
        }

        private void AwardExperience(GameObject instigator)
        {
            ExperienceLogic experience = instigator.GetComponent<ExperienceLogic>();
            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStatsLogic>().GetStat(Stat.ExperienceReward));
        }

        public void Heal(float healthToRestore)
        {
            m_health.value = Mathf.Min(m_health.value + healthToRestore, GetMaxHealthPoints());
        }

        private void RegenrateHealth()
        {
            float regenHealthPoints = GetComponent<BaseStatsLogic>().GetStat(Stat.Health) * (regenerationPercentage / 100);
            m_health.value = Mathf.Max(m_health.value, regenHealthPoints);
        }

        public float GetHealthPoints()
        {
            return m_health.value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStatsLogic>().GetStat(Stat.Health);
        }
       
        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return m_health.value / GetComponent<BaseStatsLogic>().GetStat(Stat.Health);
        }
     
        public bool IsDead()
        {
            return m_isDead;
        }
       
        // When the player levels up, it will regenerate its health by value
        private void OnEnable()
        {
            GetComponent<BaseStatsLogic>().onLevelUp += RegenrateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStatsLogic>().onLevelUp -= RegenrateHealth;
        }

        // -------------------------- //
        // ----- SAVING MODULE ------ //
        // -------------------------- //
        /// <summary>
        /// Save the player's health value
        /// </summary>
        /// <returns> When loading scene, get the health value of the player </returns>
        public object CaptureState()
        {
            return m_health.value;
        }

        public void RestoreState(object state)
        {
            m_health.value = (float)state;

            if (m_health.value == 0)
            {
                Die();
            }
        }
    }
}

