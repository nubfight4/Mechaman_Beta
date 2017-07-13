using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BacktoMMScript : MonoBehaviour {

    public void BackToMM(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
