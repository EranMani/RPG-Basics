using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerControllerLogic : MonoBehaviour
    {
        MoverLogic m_movingPlayer;
        HealthLogic m_health;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavmeshProjectionDistance = 1f;
        [SerializeField] float raycastRadius = 1f;

        void Awake()
        {
            m_movingPlayer = GetComponent<MoverLogic>();
            m_health = GetComponent<HealthLogic>();
        }

        void Update()
        {
            if (InteractWithUI()) return;

            // no further actions can be called when player is dead
            if (m_health.IsDead())
            {
                SetCurser(CursorType.None);
                return;
            }

            if (InteractWithComponent()) return;
 
            // if player clicked on an object surface to walk on
            if (InteractWithMovement()) return;

            SetCurser(CursorType.None);
        }

        private bool InteractWithUI()
        {
            // Refers only to UI objects
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCurser(CursorType.UI);
                return true;
            }

            return false;
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();

            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCurser(raycastable.GetCursorType());
                        return true;
                    }
                }
            }

            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);

            // Distances array size should be the same as the hits above
            float[] distances = new float[hits.Length];

            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            Array.Sort(distances, hits);
            return hits;
        }

        // ----------------------------------------------------------------- //
        // -------- FIND OBJECTS THAT CAN BE INTERACTED BY MOVEMENT -------- //
        // ----------------------------------------------------------------- //
        private bool InteractWithMovement()
        {
            //Debug.DrawRay(ray.origin, ray.direction * 100);
            
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);

            if (hasHit)
            {
                if (!GetComponent<MoverLogic>().CanMoveTo(target)) return false;

                if (Input.GetMouseButton(0))
                {
                    // move the player to the pointed location
                    m_movingPlayer.StartMoveAction(target, 1f);
                }
                SetCurser(CursorType.Movement);
                return true;
            }

            return false;
        }

        // This method will get a clicked position on ground and will sample a nevmesh position
        // based on a distance value. This will determine if a nearby nevmesh is found to make an area
        // available to walk on.
        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasCastToNavmesh = NavMesh.SamplePosition(
                hit.point, out navMeshHit, maxNavmeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavmesh) return false;

            target = navMeshHit.position;

            return true;
        }

        private void SetCurser(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            }

            return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            // get the ray from the camera screen point mouse position
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
