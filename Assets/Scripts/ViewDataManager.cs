using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System;

public class ViewDataManager : MonoBehaviour
{

    public GameObject Manager;
    public GameObject Persist;
    public List<Text> left_temps;
    public List<Text> right_temps;
    public Image left_foot;
    public Image right_foot;
    public Texture green;
    public Texture red;
    public Transform prefab;
    float[] foot_data_l;
    float[] foot_data_r;
    public Text message;

    float prev_press_sens_l = 0;
    float prev_press_sens_r = 0;
    bool danger_zone_l = false;
    bool danger_zone_r = false;
    bool prev_danger_zone_l = false;
    bool prev_danger_zone_r = false;

    int step_count = 0;

    float current_time;
    float time_delay;
    float progress;
    // Use this for initialization
    void Start()
    {
        Persist = GameObject.Find("Persist");
        time_delay = 0f;
        foot_data_l = new float[16];
        foot_data_r = new float[16];
    }

    // Update is called once per frame
    void Update()
    {
        if (time_delay > 0f)
        {
            time_delay -= Time.deltaTime;
            if (time_delay <= 0f)
            {
                time_delay = 0;
            }
        }
        else
        {
            message.text = step_count.ToString();
            if (Persist.GetComponent<SimpleTest>().device_left.NewData && Persist.GetComponent<SimpleTest>().device_right.NewData)
            {
                //Manager.GetComponent<TextToFile>().storeFile("[" + DateTime.UtcNow.ToString("MM-dd-yyyy HH:mm:ss:") + DateTime.UtcNow.Millisecond.ToString() + "]   left foot   " + Persist.GetComponent<SimpleTest>().device_left.DataBytes + "\r\n");
                //Manager.GetComponent<TextToFile>().storeFile("[" + DateTime.UtcNow.ToString("MM-dd-yyyy HH:mm:ss:") + DateTime.UtcNow.Millisecond.ToString() + "]   right foot   " + Persist.GetComponent<SimpleTest>().device_right.DataBytes + "\r\n");

                Manager.GetComponent<TextToFile>().storeFile(1, Persist.GetComponent<SimpleTest>().device_left.DataBytes);
                Manager.GetComponent<TextToFile>().storeFile(2, Persist.GetComponent<SimpleTest>().device_right.DataBytes);

                //left foot process data//////////////////////////////
                string[] adc_data_l = Persist.GetComponent<SimpleTest>().device_left.DataBytes.Split(',');
                if (adc_data_l[0].Contains("t"))
                {
                    switch (adc_data_l[0])
                    {
                        case "t0":
                            foot_data_l[0] = float.Parse(adc_data_l[1]);
                            foot_data_l[1] = float.Parse(adc_data_l[2]);
                            foot_data_l[2] = float.Parse(adc_data_l[3]);
                            foot_data_l[3] = float.Parse(adc_data_l[4]);
                            break;

                        case "t1":
                            foot_data_l[4] = float.Parse(adc_data_l[1]);
                            foot_data_l[5] = float.Parse(adc_data_l[2]);
                            foot_data_l[6] = float.Parse(adc_data_l[3]);
                            foot_data_l[7] = float.Parse(adc_data_l[4]);
                            break;

                        case "t2":
                            foot_data_l[8] = float.Parse(adc_data_l[1]);
                            foot_data_l[9] = float.Parse(adc_data_l[2]);
                            foot_data_l[10] = float.Parse(adc_data_l[3]);
                            foot_data_l[11] = float.Parse(adc_data_l[4]);
                            break;

                        case "t3":
                            foot_data_l[12] = float.Parse(adc_data_l[1]);
                            foot_data_l[13] = float.Parse(adc_data_l[2]);
                            foot_data_l[14] = float.Parse(adc_data_l[3]);
                            foot_data_l[15] = float.Parse(adc_data_l[4]);

                            for (int i = 0; i < 16; i++)
                            {
                                left_temps[i].text = foot_data_l[i].ToString();
                                if (i != 7)
                                {
                                    if (foot_data_l[i] > 96.8f)
                                    {
                                        danger_zone_l = true;
                                        if(!prev_danger_zone_l)
                                        {
                                            Manager.GetComponent<TimeStamp>().startTimeL = Manager.GetComponent<TimeStamp>().t;
                                            Persist.GetComponent<DataArray>().dataArrayL[1, 1] = "True";
                                            Persist.GetComponent<DataArray>().dataArrayL[2, 1] = Manager.GetComponent<TimeStamp>().startTimeL.ToString();
                                        }

                                        left_temps[i].color = new Color32(0xFF, 0x00, 0x00, 0xFF);      //red : FF0000FF
                                        Handheld.Vibrate();
                                    }
                                    else
                                    {
                                        left_temps[i].color = new Color32(0x05, 0xCB, 0x15, 0xFF);      //grean : 05CB15FF
                                    }
                                }
                                else
                                {
                                    if (foot_data_l[i] > 100.0f && prev_press_sens_l < 100)
                                    {
                                        step_count++;
                                        left_foot.GetComponent<Image>().color = new Color32(0xBF, 0xBF, 0xBF, 0xFF);
                                    }
                                    else
                                    {
                                        left_foot.GetComponent<Image>().color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
                                    }
                                    prev_press_sens_l = foot_data_l[i];
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }

                //right foot process data//////////////////////////////
                string[] adc_data_r = Persist.GetComponent<SimpleTest>().device_right.DataBytes.Split(',');
                if (adc_data_r[0].Contains("t"))
                {
                    switch (adc_data_r[0])
                    {
                        case "t0":
                            foot_data_r[0] = float.Parse(adc_data_r[1]);
                            foot_data_r[1] = float.Parse(adc_data_r[2]);
                            foot_data_r[2] = float.Parse(adc_data_r[3]);
                            foot_data_r[3] = float.Parse(adc_data_r[4]);
                            break;

                        case "t1":
                            foot_data_r[4] = float.Parse(adc_data_r[1]);
                            foot_data_r[5] = float.Parse(adc_data_r[2]);
                            foot_data_r[6] = float.Parse(adc_data_r[3]);
                            foot_data_r[7] = float.Parse(adc_data_r[4]);
                            break;

                        case "t2":
                            foot_data_r[8] = float.Parse(adc_data_r[1]);
                            foot_data_r[9] = float.Parse(adc_data_r[2]);
                            foot_data_r[10] = float.Parse(adc_data_r[3]);
                            foot_data_r[11] = float.Parse(adc_data_r[4]);
                            break;

                        case "t3":
                            foot_data_r[12] = float.Parse(adc_data_r[1]);
                            foot_data_r[13] = float.Parse(adc_data_r[2]);
                            foot_data_r[14] = float.Parse(adc_data_r[3]);
                            foot_data_r[15] = float.Parse(adc_data_r[4]);

                            for (int i = 0; i < 16; i++)
                            {
                                right_temps[i].text = foot_data_r[i].ToString();
                                if (i != 7)
                                {
                                    if (foot_data_r[i] > 96.8f)
                                    {
                                        danger_zone_r = true;

                                        if (!prev_danger_zone_r)
                                        {
                                            Manager.GetComponent<TimeStamp>().startTimeR = Manager.GetComponent<TimeStamp>().t;
                                            Persist.GetComponent<DataArray>().dataArrayR[1, 1] = "True";
                                            Persist.GetComponent<DataArray>().dataArrayR[2, 1] = Manager.GetComponent<TimeStamp>().startTimeR.ToString();
                                        }

                                        right_temps[i].color = new Color32(0xFF, 0x00, 0x00, 0xFF);      //red : FF0000FF
                                        Handheld.Vibrate();
                                    }
                                    else
                                    {
                                        right_temps[i].color = new Color32(0x05, 0xCB, 0x15, 0xFF);      //grean : 05CB15FF
                                    }
                                }
                                else
                                {
                                    if (foot_data_r[i] > 100.0f && prev_press_sens_r < 100)
                                    {
                                        step_count++;
                                        right_foot.GetComponent<Image>().color = new Color32(0xBF, 0xBF, 0xBF, 0xFF);
                                    }
                                    else
                                    {
                                        right_foot.GetComponent<Image>().color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
                                    }
                                    prev_press_sens_r = foot_data_r[i];
                                }
                            }
                            //Persist.GetComponent<SimpleTest>().SendBytes(System.Text.Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("HH:mm:ss:") + DateTime.UtcNow.Millisecond.ToString() + "\r\n"));
                            break;

                        default:
                            break;
                    }
                }

                for (int i = 0; i < 16; i++)
                {
                    if (i != 7)
                    {
                        if ((foot_data_l[i] - foot_data_r[i]) > 4)
                        {
                            danger_zone_l = true;

                            if (!prev_danger_zone_l)
                            {
                                Manager.GetComponent<TimeStamp>().startTimeL = Manager.GetComponent<TimeStamp>().t;
                                Persist.GetComponent<DataArray>().dataArrayL[1, 1] = "True";
                                Persist.GetComponent<DataArray>().dataArrayL[2, 1] = Manager.GetComponent<TimeStamp>().startTimeL.ToString();
                            }

                            left_temps[i].color = new Color32(0xFF, 0x00, 0x00, 0xFF);      //red : FF0000FF
                        }
                        else if ((foot_data_r[i] - foot_data_l[i]) > 4)
                        {
                            danger_zone_r = true;

                            if (!prev_danger_zone_r)
                            {
                                Manager.GetComponent<TimeStamp>().startTimeR = Manager.GetComponent<TimeStamp>().t;
                                Persist.GetComponent<DataArray>().dataArrayR[1, 1] = "True";
                                Persist.GetComponent<DataArray>().dataArrayR[2, 1] = Manager.GetComponent<TimeStamp>().startTimeR.ToString();
                            }

                            right_temps[i].color = new Color32(0xFF, 0x00, 0x00, 0xFF);      //red : FF0000FF
                        }
                    }
                }

                prev_danger_zone_l = danger_zone_l;
                prev_danger_zone_r = danger_zone_r;

                if(danger_zone_l)
                {
                    Persist.GetComponent<SimpleTest>().SendByteL((byte)0x32);
                    Handheld.Vibrate();
                    danger_zone_l = false;
                }
                else
                {
                    Persist.GetComponent<SimpleTest>().SendByteL((byte)0x33);
                    Persist.GetComponent<DataArray>().dataArrayL[1, 1] = "False";
                    Persist.GetComponent<DataArray>().dataArrayL[2, 1] = "0";
                }

                if(danger_zone_r)
                {
                    Persist.GetComponent<SimpleTest>().SendByteR((byte)0x32);
                    Handheld.Vibrate();
                    danger_zone_r = false;
                }
                else
                {
                    Persist.GetComponent<SimpleTest>().SendByteR((byte)0x33);
                    Persist.GetComponent<DataArray>().dataArrayR[1, 1] = "False";
                    Persist.GetComponent<DataArray>().dataArrayR[2, 1] = "0";
                }

                Persist.GetComponent<SimpleTest>().device_left.NewData = false;
                Persist.GetComponent<SimpleTest>().device_right.NewData = false;
            }
        }
    }
}
