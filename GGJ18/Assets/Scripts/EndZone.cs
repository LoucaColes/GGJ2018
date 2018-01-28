using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider _other)
    {
        if (_other.gameObject.CompareTag("Human"))
        {
            HumanManager.Instance.HumanSafe(_other.gameObject.GetComponent<Human>());
            GameManager.m_instance.IncreaseScore(1);
        }
    }
}