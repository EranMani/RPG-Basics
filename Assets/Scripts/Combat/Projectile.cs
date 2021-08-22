using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float m_speed = 1f;
        [SerializeField] bool isHoming = true;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10.0f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 2f;
        [SerializeField] UnityEvent onHit;

        HealthLogic m_target = null;
        GameObject instigator = null;
        float damage = 0;


        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        // Update is called once per frame
        void Update()
        {
            if (m_target == null) return;

            if (isHoming && !m_target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }

            transform.Translate(Vector3.forward * m_speed * Time.deltaTime);
        }

        public void SetTarget(HealthLogic target, GameObject instigator, float damage)
        {
            this.m_target = target;
            this.damage = damage;
            this.instigator = instigator;

            Destroy(this.gameObject, maxLifeTime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = m_target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return m_target.transform.position;
            }

            return m_target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<HealthLogic>() != m_target) return;
            if (m_target.IsDead()) return;
            m_target.TakeDamage(instigator, damage);

            m_speed = 0;

            onHit.Invoke();

            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(this.gameObject, lifeAfterImpact);
        }
    }
}
