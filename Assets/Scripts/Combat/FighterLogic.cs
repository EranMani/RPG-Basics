using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using System;

// ------------------------------------------------------------------------------------------------------ //
// THIS CLASS WILL ALLOW FOR THE OBJECT TO ATTACK
// IT WILL REGISTER AS AN ACTION TO THE SCHEDULAR CLASS, THUS CANCELLING MOVEMENT
// IT WILL DETERMINE WHEN THE OBJECT CAN ATTACK AND WILL DEAL WITH THE ATTACK BEHAVIOURS
// ------------------------------------------------------------------------------------------------------ //

namespace RPG.Combat
{
    public class FighterLogic : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {      
        [SerializeField] float m_timeBetweenAttacks = 1f;
        [SerializeField] Transform m_rightHandTransform = null;
        [SerializeField] Transform m_leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
        
        HealthLogic target;
        float m_timeSinceLastAttack = Mathf.Infinity;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        ActionSchedularLogic m_actionSchedular;
        Animator m_anim;

        private void Awake()
        {
            m_actionSchedular = GetComponent<ActionSchedularLogic>();
            m_anim = GetComponent<Animator>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            m_timeSinceLastAttack += Time.deltaTime;
            
            // cancel the fight ability when target is not available
            if (target == null) return;
            if (target.IsDead()) return;

            FightActionBasedOnRange();
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.GetWeaponRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }
            if (!GetComponent<MoverLogic>().CanMoveTo(combatTarget.transform.position)) return false;

            HealthLogic targetCheck = combatTarget.GetComponent<HealthLogic>();
            return targetCheck != null && !targetCheck.IsDead();
        }

        // this function will be called by the player or enemy //
        public void Attack(GameObject combatTarget)
        {
            // register the attack action to the action schedular
            m_actionSchedular.StartAction(this);

            // assign new target
            target = combatTarget.GetComponent<HealthLogic>();
        }

        private void FightActionBasedOnRange()
        {
            bool isInRange = Vector3.Distance(transform.position, target.transform.position) < currentWeaponConfig.GetWeaponRange();

            if (!isInRange)
            {
                GetComponent<MoverLogic>().MoveTo(target.transform.position, 1f);
            }
            else
            {
                GetComponent<MoverLogic>().Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            // point the attacker to the target direction
            transform.LookAt(target.transform.position);

            // when the ability to attack has restored
            if (m_timeSinceLastAttack > m_timeBetweenAttacks)
            {
                // tigger the 'Hit()' animation event

                // handle animations
                TriggerAttack();

                // reset the ability to attack 
                m_timeSinceLastAttack = 0;
            }
        }

        // Animation event
        public void Hit()
        {
            if (target == null) return;

            float damage = GetComponent<BaseStatsLogic>().GetStat(Stat.Damage);

            if (currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }

            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(m_rightHandTransform, m_leftHandTransform, target, gameObject, damage);
            }
            else
            {
                // --call to the 'HealthLogic' script-- //
                target.TakeDamage(gameObject, damage);
            }
            
        }

        public void Shoot()
        {
            Hit();
        }

        private void TriggerAttack()
        {
            m_anim.ResetTrigger("stopAttack");
            m_anim.SetTrigger("attack");
        }
    
        public void Cancel()
        {
            // this function will stop the attack animations from playing
            // it will declare the current target as not available anymore
            // it will cancel the player movement as well
            StopAttack(); 
            target = null;
            GetComponent<MoverLogic>().Cancel();
        }

        private void StopAttack()
        {
            m_anim.ResetTrigger("attack");
            m_anim.SetTrigger("stopAttack");
        }

        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            // change the current weapon to the one that is equipped
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(m_rightHandTransform, m_leftHandTransform, m_anim);
        }

        public HealthLogic GetTarget()
        {
            return target;
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            // find related files with the type WEAPON in the resources folder and search for a specific object by name
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }
}

