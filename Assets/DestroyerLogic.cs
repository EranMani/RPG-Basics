using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerLogic : MonoBehaviour
{
    [SerializeField] GameObject targetToDestroy = null;

    public void DestroyTarget()
    {
        Destroy(targetToDestroy);
    }
}
