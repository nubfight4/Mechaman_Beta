using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Text_Importer : MonoBehaviour
{

    public TextAsset textFile;
    public string[] textLine;

    // Use this for initialization
    void Start()
    {
        if (textFile != null)
        {
            textLine = (textFile.text.Split('\n'));
        }
    }


}
