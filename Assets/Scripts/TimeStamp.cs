using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimeStamp : MonoBehaviour
{

    public DateTime epoch;
    public int t;

    public int startTimeL;
    public int startTimeR;

    public int since_start_l;
    public float since_start_r;

    public bool long_term_left_started = false;
    public bool long_term_right_started = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        t = (int)(DateTime.UtcNow - epoch).TotalSeconds;

        if (long_term_left_started)
        {
            since_start_l = t - startTimeL;
        }

        if (long_term_right_started)
        {
            since_start_r = t - startTimeR;
        }
    }
}
