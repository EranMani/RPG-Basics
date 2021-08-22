using UnityEngine;
using RPG.Attributes;
using RPG.Control;

/// <summary>
/// CombatTargetLogic will make the holder a target. This script needs to make sure that the holder
/// is a character by having the health component on it
/// The holder will also be able to be hit with a ray, and adjust the cursor type as well
/// </summary>
namespace RPG.Combat
{
    [RequireComponent(typeof(HealthLogic))]
    public class CombatTargetLogic : MonoBehaviour, IRaycastable
    {      
        public bool HandleRaycast(PlayerControllerLogic callingController)
        {
            if (!callingController.GetComponent<FighterLogic>().CanAttack(gameObject)) return false;

            if (Input.GetMouseButton(0))
            {
                callingController.GetComponent<FighterLogic>().Attack(gameObject);
            }

            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }
    }
}

