using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using GameDevTV.Utils;
using System;

namespace RPG.Control
{
    public class AIControllerLogic : MonoBehaviour
    {
        [SerializeField] float m_chaseDistance = 5f;
        [SerializeField] float m_suspicionTime = 5f;
        [SerializeField] float agroCoolDownTime = 5f;
        [SerializeField] float m_wayPointTolerance = 1f;
        [SerializeField] float m_wayPointDwellTime = 3f;
        [SerializeField] PatrolPathLogic m_patrolPath = null;
        [Range(0,1)]
        [SerializeField] float m_patrolSpeedFraction = 0.2f;
        [SerializeField] float shoutDistance = 5f;
        
        FighterLogic m_fighter;     
        HealthLogic m_health;
        MoverLogic m_mover;
        GameObject m_player;

        float m_timeSinceLastSawPlayer = Mathf.Infinity;
        float m_timeSinceArrivedAtWaypoint = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;
        int m_currentWaypointIndex = 0;

        LazyValue<Vector3> m_guardPosition;

        private void Awake()
        {
            m_player = GameObject.FindWithTag("Player");
            m_fighter = GetComponent<FighterLogic>();
            m_health = GetComponent<HealthLogic>();
            m_mover = GetComponent<MoverLogic>();

            m_guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Start()
        {
            m_guardPosition.ForceInit();
        }

        private void Update()
        {
            if (m_health.IsDead()) return;

            if (IsAggrevated() && m_fighter.CanAttack(m_player))
            {            
                AttackBehaviour();
            }
            else if (m_timeSinceLastSawPlayer < m_suspicionTime)
            {
                // stand in place and cancel other actions
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        public void Aggrevate()
        {
            timeSinceAggrevated = 0;
        }

        private bool IsAggrevated()
        {
            float distanceToPlayer = Vector3.Distance(m_player.transform.position, transform.position);
            return distanceToPlayer < m_chaseDistance || timeSinceAggrevated < agroCoolDownTime;
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionSchedularLogic>().CancelCurrentAction();
        }
  
        private void PatrolBehaviour()
        {
            // when no patrol path assigned, go back to original position
            Vector3 nextPosition = m_guardPosition.value;

            if (m_patrolPath != null)
            {
                // when reached a waypoint
                if (AtWayPoint())
                {
                    // reset the var to 0
                    m_timeSinceArrivedAtWaypoint = 0;
                    // calculate the movement to the next waypoint
                    CycleWayPoint();
                }

                nextPosition = GetCurrentWaypoint();
            }

            // if dwell time passed, go to the next waypoint
            if (m_timeSinceArrivedAtWaypoint > m_wayPointDwellTime)
            {
                m_mover.StartMoveAction(nextPosition, m_patrolSpeedFraction);
            }
            
        }
    
        private Vector3 GetCurrentWaypoint()
        {
            return m_patrolPath.GetWayPoint(m_currentWaypointIndex);
        }

        private void CycleWayPoint()
        {
            m_currentWaypointIndex = m_patrolPath.GetNextIndex(m_currentWaypointIndex);
        }

        private bool AtWayPoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < m_wayPointTolerance;
        }

        private void UpdateTimers()
        {
            m_timeSinceLastSawPlayer += Time.deltaTime;
            m_timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }



        private void AttackBehaviour()
        {   
            // reset the var to 0
            m_timeSinceLastSawPlayer = 0;
            m_fighter.Attack(m_player);

            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits =  Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                AIControllerLogic ai = hit.collider.GetComponent<AIControllerLogic>();
                if (ai == null) continue;
                
                ai.Aggrevate();            
            }
        }



        // called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, m_chaseDistance);
        }
    }

    
}

