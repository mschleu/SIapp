using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class SeachingForDevices : MonoBehaviour {

    public List<Image> Reds;
    public List<Image> Blues;
    private bool circle_state = false;

    // Use this for initialization
    void Start () {
        int i = 0;
        foreach (var circle in Reds)
        {
            Reds[i].enabled = circle_state;
            Blues[i].enabled = circle_state;
            i++;
            if (i == 4)
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
        int i = 0;
        foreach (var circle in Reds)
        {
            Reds[i].enabled = !circle_state;
            i++;
            if (i == 4)
            {
                i = 0;
                circle_state = !circle_state;
            }
        }
    }
}
