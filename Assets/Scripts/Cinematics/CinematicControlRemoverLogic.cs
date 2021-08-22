using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

/// <summary>
/// CinematicControlRemoverLogic will remove control from the player, making him static during a cinematic
/// </summary>
namespace RPG.Cinematics
{
    public class CinematicControlRemoverLogic : MonoBehaviour
    {
        GameObject m_player;

        private void Awake()
        {
            m_player = GameObject.FindWithTag("Player");          
        }

        void DisableControl(PlayableDirector pd)
        {
            m_player.GetComponent<ActionSchedularLogic>().CancelCurrentAction();
            m_player.GetComponent<PlayerControllerLogic>().enabled = false;
            print("DisableControl");
        }

        void EnableControl(PlayableDirector pd)
        {
            m_player.GetComponent<PlayerControllerLogic>().enabled = true;
            print("EnableControl");
        }

        // Register methods to director events and unregister when script is destroyed
        private void OnEnable()
        {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
        }

        private void OnDisable()
        {
            GetComponent<PlayableDirector>().played -= DisableControl;
            GetComponent<PlayableDirector>().stopped -= EnableControl;
        }      
    }
}

