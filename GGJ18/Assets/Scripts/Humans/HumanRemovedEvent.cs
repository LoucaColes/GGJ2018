using UnityEngine;

public class HumanRemovedEvent
{
    public Transform Human;

    public HumanRemovedEvent(Transform _human)
    {
        Human = _human;
    }
}