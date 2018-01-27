using UnityEngine;
using System.Collections;

[System.Serializable]
public class CTO
{
    public string objectName;
    public Color colour;
    public GameObject aObject;
}

[CreateAssetMenu(fileName = "ColourToObject.asset", menuName = "LevelEditor/CTO")]
public class ColourToObject : ScriptableObject
{
    public CTO[] colourToObject;
}