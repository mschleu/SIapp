using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneNum : MonoBehaviour
{

    // Use this for initialization
    public void loadThisScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }
}
