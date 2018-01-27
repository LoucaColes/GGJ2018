using UnityEngine;

public class HumanRemovedEvent
{
    public Transform Human;
    public int HumanIndex;

    public HumanRemovedEvent(Transform _human, int _humanIndex)
    {
        Human = _human;
        HumanIndex = _humanIndex;
    }
}