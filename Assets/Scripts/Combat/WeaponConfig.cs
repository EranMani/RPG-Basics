using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make_New_Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController m_animatorOverride = null;
        [SerializeField] Weapon m_equippedPrefab = null;
        [SerializeField] float m_weaponDamage = 5f;
        [SerializeField] float percentageBonus = 0f;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        // every new weapon that is picked should be named as the string below
        const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            Weapon weapon = null;

            if (m_equippedPrefab !=  null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                weapon =  Instantiate(m_equippedPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (m_animatorOverride != null)
            {
                // this override controller allows to override a different animation that will replace
                // the original animation that was assigned
                animator.runtimeAnimatorController = m_animatorOverride;
            }
            else if (overrideController != null)
            {
                // in case the animator has been overriden, find its parent (character animator)
                // assign the parent animator as the current runtime animator to run the default 
                // animations
                // in case the default animator has not been overriden, change nothing, because 
                // the animator remembers the last animator that ran.
                animator.runtimeAnimatorController = m_animatorOverride.runtimeAnimatorController;
            }

            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            // if we find a game object in the right hand, that is a weapon
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;

            // rename the old weapon so that the name wont collide with the new picked weapon
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded) handTransform = rightHand;
            else handTransform = leftHand;
            return handTransform;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, HealthLogic target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        public float GetWeaponDamage()
        {
            return m_weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public float GetWeaponRange()
        {
            return weaponRange;
        }
    }
}
