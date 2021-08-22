using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------------------------------------------ //
// THIS CLASS WILL RECORD CURRENT PLAYER ACTIONS
// IT WILL SWAP BETWEEN THE "CANCEL" FUNCTION OF EACH ACTION WHEN ANOTHER IS BEING PERFORMED
// EACH ACTION WILL REPORT TO THIS SCHEDULAR WHEN ACTIVATED AND THE SCHEDULAR WILL CANCEL THE OTHER ACTION
// ------------------------------------------------------------------------------------------------------ //
// *** Substitution principle *** //
// dependency inversion:  Schedular -> IAction (interface) -> Movement or Combat //

namespace RPG.Core
{
    public class ActionSchedularLogic : MonoBehaviour
    {            
        // get actions which has a contract with the IAction script
        IAction m_currentAction;

        public void StartAction(IAction action)
        {    
            // do nothing when the same action is being reported
            if (m_currentAction == action) return;
            
            // when a different action then the stored is reported, run its Cancal function
            if (m_currentAction != null)
            {
                m_currentAction.Cancel();
            }

            // store the reported action to current
            m_currentAction = action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}


