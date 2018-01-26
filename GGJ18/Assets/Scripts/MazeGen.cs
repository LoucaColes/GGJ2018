using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MazeGen : MonoBehaviour
{
    public Texture2D map;
    public ColourToObject mappings;

    public string levelName;
    public string exportPath;

    private GameObject m_parent;

    private void Start()
    {
        m_parent = new GameObject(levelName);
        m_parent.transform.position = Vector3.zero;
        ExportLevelData();
    }

    public void ExportLevelData()
    {
        for (int y = 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                if (map.GetPixel(x, y).a != 0)
                {
                    Debug.Log("Tile " + x + " " + y + " Colour: " + map.GetPixel(x, y));
                    //valid pixel
                    foreach (CTO objects in mappings.colourToObject)
                    {
                        //if colours match
                        if (CheckColours(map.GetPixel(x, y), objects.colour))
                        {
                            // create object
                            Vector3 t_pos = new Vector3(x, 1f, y);
                            GameObject t_object = (GameObject)Instantiate(objects.aObject, t_pos, Quaternion.identity);
                            t_object.transform.parent = m_parent.transform;
                        }
                    }
                }
            }
        }

        //export data
        string t_fileName = levelName + ".asset";
        string t_filePath = exportPath + t_fileName;
        //AssetDatabase.CreateAsset(m_parent, t_filePath);
    }

    private bool CheckColours(Color _a, Color _b)
    {
        int colourAR = Mathf.RoundToInt(_a.r * 255);
        int colourAG = Mathf.RoundToInt(_a.g * 255);
        int colourAB = Mathf.RoundToInt(_a.b * 255);

        int colourBR = Mathf.RoundToInt(_b.r * 255);
        int colourBG = Mathf.RoundToInt(_b.g * 255);
        int colourBB = Mathf.RoundToInt(_b.b * 255);

        if ((colourAR == colourBR) && (colourAG == colourBG) && (colourAB == colourBB))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}