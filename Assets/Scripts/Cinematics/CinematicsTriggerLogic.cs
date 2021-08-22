using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// CinematicsTriggerLogic will trigger the moment of a cinematic
/// </summary>
namespace RPG.Cinematics
{
    public class CinematicsTriggerLogic : MonoBehaviour
    {
        bool m_playedOnce = false;

        private void OnTriggerEnter(Collider other)
        {
            // Play the cinematic shot only once while the game is on
            if (other.tag == "Player" && !m_playedOnce)
            {
                m_playedOnce = true;
                GetComponent<PlayableDirector>().Play();
            }          
        }
    }
}

