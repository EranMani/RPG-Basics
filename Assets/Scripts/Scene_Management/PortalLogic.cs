using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using UnityEngine.AI;
using RPG.Control;

// this class will use the saving system when crossing between portals 
// it will also use the portals as a checkpoint systems
namespace RPG.SceneManagement
{
    public class PortalLogic : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C
        }

        [SerializeField] int m_sceneToLoad = -1;
        [SerializeField] Transform m_spawnPoint;
        [SerializeField] DestinationIdentifier m_destination;
        [SerializeField] float m_fadeOutTime = 1f;
        [SerializeField] float m_fadeInTime = 2f;
        [SerializeField] float m_fadeWaitTime = 0.5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(Transition());    
            }         
        }

        private IEnumerator Transition() 
        {
            if (m_sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set!");
                yield break;
            }
        
            DontDestroyOnLoad(this.gameObject);

            FaderLogic fader = FindObjectOfType<FaderLogic>();
            // before moving to a new scene, save the current state of the scene
            // gather all the saveable entities and store them
            SavingWrapperLogic wrapper = FindObjectOfType<SavingWrapperLogic>();
            PlayerControllerLogic playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerLogic>();
            playerController.enabled = false;

            yield return fader.FadeOut(m_fadeOutTime);
               
            wrapper.Save();

            yield return SceneManager.LoadSceneAsync(m_sceneToLoad);
            PlayerControllerLogic newPlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerLogic>();
            newPlayerController.enabled = false;

            wrapper.Load();

            PortalLogic otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            // this should help do an automatic save between scenes after loading stats and getting
            // last player position
            // this will make sure that the game will return to the saved scene when activated
            // in case there is no save file, or the save file is empty because the user didnt saved before
            // the program should save the player location after being teleported, so that the next time
            // he will open the game - the player will be at the position of that teleported location - which
            // was saved automatically into the save file after the teleport is done.
            wrapper.Save();


            yield return new WaitForSeconds(m_fadeWaitTime);
            fader.FadeIn(m_fadeInTime);

            newPlayerController.enabled = true;
            Destroy(this.gameObject);
        }

        private void UpdatePlayer(PortalLogic otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.m_spawnPoint.position);
            player.transform.rotation = otherPortal.m_spawnPoint.rotation;
        }

        private PortalLogic GetOtherPortal()
        {
            foreach(PortalLogic portal in FindObjectsOfType<PortalLogic>())
            {
                if (portal == this) continue;
                if (portal.m_destination != m_destination) continue;

                return portal;
            }

            return null;
        }
    }
}

