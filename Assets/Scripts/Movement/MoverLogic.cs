using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

// ------------------------------------------------------------------------------------------------------ //
// THIS CLASS WILL ALLOW THE OBJECT TO MOVE ON NAV MESH SURFACE
// IT WILL REGISTER TO THE SCHEDULAR AS A MOVEMENT ACTION, THUS CANCELLING THE FIGHTING ACTION
// IT WILL ALLOW THE OBJECT TO MOVE TO OTHER DESTINATIONS
// ------------------------------------------------------------------------------------------------------ //

namespace RPG.Movement
{
    public class MoverLogic : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float m_maxSpeed = 6f;
        [SerializeField] float maxNavPathLength = 40f;
        HealthLogic m_health;

        ActionSchedularLogic m_actionSchedular;
        
        NavMeshAgent m_navAgent;
        Animator m_anim;

        private void Awake()
        {
            m_navAgent = GetComponent<NavMeshAgent>();
            m_anim = GetComponent<Animator>();
            m_actionSchedular = GetComponent<ActionSchedularLogic>();
            m_health = GetComponent<HealthLogic>();
        }

        void Update()
        {
            // object can move while not dead
            m_navAgent.enabled = !m_health.IsDead();

            UpdateAnimator();
        }

        // this function will be called by the player or enemy //
        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            m_actionSchedular.StartAction(this);         
            MoveTo(destination, speedFraction);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxNavPathLength) return false;

            return true;
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            m_navAgent.destination = destination;
            m_navAgent.speed = m_maxSpeed * Mathf.Clamp01(speedFraction);
            m_navAgent.isStopped = false;
        }

        public void Cancel()
        {
            m_navAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            // get object velocity from agent
            Vector3 velocity = m_navAgent.velocity;

            // "InverseTransformDirection" will convert the global (world position) velocity that we are getting from the navmesh 
            // to local velocity that the animator can work with
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;

            m_anim.SetFloat("forwardSpeed", speed);
        }

        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }


        // -------------------------- //
        // ----- SAVING MODULE ------ //
        // -------------------------- //
        [System.Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }

        public object CaptureState()
        {
            //Dictionary<string, object> data = new Dictionary<string, object>();
            //data["position"] = new SerializableVector3(transform.position);
            //data["rotation"] = new SerializableVector3(transform.eulerAngles);
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.eulerAngles);
            // return new SerializableVector3(transform.position); -> when return single data <-
            return data;
        }

        public void RestoreState(object state)
        {
            // Dictionary<string, object> data = (Dictionary<string, object>) state;
            //transform.position = ((SerializableVector3)data["position"]).ToVector();
            // transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();
            MoverSaveData data = (MoverSaveData) state;
            m_navAgent.enabled = false;
            transform.position = data.position.ToVector();
            transform.eulerAngles = data.rotation.ToVector();         
            m_navAgent.enabled = true;
        }
    }
}


