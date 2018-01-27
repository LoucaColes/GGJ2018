using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndZone : MonoBehaviour
{
    void OnTriggerEnter(Collider _other)
    {
        if(_other.gameObject.CompareTag("Human"))
        {
            HumanManager.Instance.HumanSafe(_other.gameObject.GetComponent<Human>());
        }
    }
}
