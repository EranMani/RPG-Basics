using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{   public class FollowCameraLogic : MonoBehaviour
    {
        [SerializeField]
        Transform m_target;

        // Update is called once per frame
        void LateUpdate()
        {
            transform.position = m_target.position;
        }
    }
}

