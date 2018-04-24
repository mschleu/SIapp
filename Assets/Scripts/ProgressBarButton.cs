using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

public class ProgressBarButton : MonoBehaviour
{
    public GameObject LoadingBar;
    public GameObject Persist;
    public Text mButton;

    float startTime;
    float endTime;
    float currentTime;
    float progress;

    bool devices_found = false;
    bool pressed = false;

    public int sceneSelect;

    private void Start()
    {
        Persist = GameObject.Find("Persist");
        LoadingBar.GetComponent<Image>().fillAmount = 0;
    }

    void Update()
    {
        if(Persist.GetComponent<SimpleTest>()._connected)
        {
            mButton.text = "Pair found,\n hold to continue";
            currentTime = Time.time;
            if (pressed)
            {
                progress = currentTime - startTime;
                LoadingBar.GetComponent<Image>().fillAmount = progress;
                //mSlider.value = progress;
                if (progress >= 1)
                {
                    Persist.GetComponent<SimpleTest>().start_button();
                    SceneManager.LoadScene(sceneSelect);
                }
            }
            else
            {
                //mSlider.value = 0;
                LoadingBar.GetComponent<Image>().fillAmount = 0;
            }
        }
    }

    public void buttonDown()
    {
        startTime = Time.time;
        pressed = true;
    }

    public void buttonUp()
    {
        pressed = false;
    }
}
