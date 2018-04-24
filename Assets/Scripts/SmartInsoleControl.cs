using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

public class SmartInsoleControl : MonoBehaviour {

    public GameObject Manager;
    public GameObject Persist;
    public Button start_button;
    public Button transfer_button;
    public Button view_button;

    // Use this for initialization
    void Start ()
    {
        Persist = GameObject.Find("Persist");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void smart_insole_init()
    {
        Persist.GetComponent<SimpleTest>().start_button();
        start_button.interactable = false;
        view_button.interactable = true;
    }
}
