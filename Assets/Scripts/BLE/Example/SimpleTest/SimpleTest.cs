/* This is a simple example to show the steps and one possible way of
 * automatically scanning for and connecting to a device to receive
 * notification data from the device.
 */

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class SimpleTest : MonoBehaviour
{
    public GameObject manager;
    public string ServiceUUID = "0001";
    public string SubscribeCharacteristic = "0003";
    public string WriteCharacteristic = "0002";
    //private int num_devices = 0;

    public bool _connected = false;
    public float _timeout = 0f;
    public States _state = States.None;
    public string _deviceAddress;
    public bool _foundSubscribeID = false;
    public bool _foundWriteID = false;
    public byte[] _dataBytes = null;

    public enum States
    {
        None,
        Scan,
        //ScanRSSI,
        Connect,
        Subscribe,
        Unsubscribe,
        Disconnect,
    }

    public enum DEVICE_STATE
    {
        STARTUP = 0,
        FOUND,
        CONNECTED,
        SUBSCRIBED,
    }

    public struct DEVICE_OBJECT
    {
        public DEVICE_STATE State;
        public string Address;
        public string Name;
        public bool FoundSubscribeID;
        public bool FoundWriteID;
        public bool Initialized;
        public bool NewData;
        public string DataBytes;
    }

    public DEVICE_OBJECT device_left;
    public DEVICE_OBJECT device_right;

    void Reset()
    {
        //reset left foot device
        device_left.State = DEVICE_STATE.STARTUP;
        device_left.FoundSubscribeID = false;
        device_left.FoundWriteID = false;
        device_left.Initialized = false;
        device_left.NewData = false;
        device_left.DataBytes = null;

        //eset right foot device
        device_right.State = DEVICE_STATE.STARTUP;
        device_right.FoundSubscribeID = false;
        device_right.FoundWriteID = false;
        device_right.Initialized = false;
        device_right.NewData = false;
        device_right.DataBytes = null;

        //initialize states
        _connected = false;
        _timeout = 0f;
        _state = States.None;
        _deviceAddress = null;
        _foundSubscribeID = false;
        _foundWriteID = false;
        _dataBytes = null;
    }

    void SetState(States newState, float timeout)
    {
        _state = newState;
        _timeout = timeout;
    }

    public void StartProcess()
    {
        Reset();
        BluetoothLEHardwareInterface.Initialize(true, false, () => {

            SetState(States.Scan, 0.1f);

        }, (error) => {

            BluetoothLEHardwareInterface.Log("Error during initialize: " + error);
        });
    }

    //Use this for initialization
    void Start()
    {
        StartProcess();
    }

    // Update is called once per frame
    void Update()
    {
        if (_timeout > 0f)
        {
            _timeout -= Time.deltaTime;
            if (_timeout <= 0f)
            {
                _timeout = 0f;

                switch (_state)
                {
                    case States.None:
                        break;

                    case States.Scan:
                        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) => {

                            if (name == "SmartInsoleL")
                            {
                                device_left.Name = name;
                                device_left.Address = address;
                                device_left.State = DEVICE_STATE.FOUND;
                            }
                            else if(name == "SmartInsoleR")
                            {
                                device_right.Name = name;
                                device_right.Address = address;
                                device_right.State = DEVICE_STATE.FOUND;
                            }

                            if(device_right.State == DEVICE_STATE.FOUND && device_left.State == DEVICE_STATE.FOUND)
                            {
                                BluetoothLEHardwareInterface.StopScan();
                                SetState(States.Connect, 0.5f);
                            }

                        }, null);

                        break;

                    case States.Connect:
                        BluetoothLEHardwareInterface.ConnectToPeripheral(device_left.Address, null, null, (address, serviceUUID, characteristicUUID) => {

                            if (IsEqual(serviceUUID, ServiceUUID))
                            {
                                device_left.FoundSubscribeID = device_left.FoundSubscribeID || IsEqual(characteristicUUID, SubscribeCharacteristic);
                                device_left.FoundWriteID = device_left.FoundWriteID || IsEqual(characteristicUUID, WriteCharacteristic);

                                // if we have found both characteristics that we are waiting for
                                // set the state. make sure there is enough timeout that if the
                                // device is still enumerating other characteristics it finishes
                                // before we try to subscribe
                                if (device_left.FoundSubscribeID && device_left.FoundWriteID)
                                {
                                    device_left.State = DEVICE_STATE.CONNECTED;
                                    SetState(States.Subscribe, 0.5f);
                                }
                            }
                        });

                        BluetoothLEHardwareInterface.ConnectToPeripheral(device_right.Address, null, null, (address, serviceUUID, characteristicUUID) => {

                            if (IsEqual(serviceUUID, ServiceUUID))
                            {
                                device_right.FoundSubscribeID = device_right.FoundSubscribeID || IsEqual(characteristicUUID, SubscribeCharacteristic);
                                device_right.FoundWriteID = device_right.FoundWriteID || IsEqual(characteristicUUID, WriteCharacteristic);

                                // if we have found both characteristics that we are waiting for
                                // set the state. make sure there is enough timeout that if the
                                // device is still enumerating other characteristics it finishes
                                // before we try to subscribe
                                if (device_right.FoundSubscribeID && device_right.FoundWriteID)
                                {
                                    device_right.State = DEVICE_STATE.CONNECTED;
                                    SetState(States.Subscribe, 0.5f);
                                }
                            }
                        });

                        break;

                    case States.Subscribe:
                        BluetoothLEHardwareInterface.SubscribeCharacteristic(device_left.Address, FullUUID(ServiceUUID), FullUUID(SubscribeCharacteristic), null, (characteristic, bytes) =>
                        {
                            //string s = ASCIIEncoding.UTF8.GetString(bytes);
                            //left_foot_data.text = s;
                            device_left.DataBytes = ASCIIEncoding.UTF8.GetString(bytes);
                            device_left.State = DEVICE_STATE.SUBSCRIBED;
                            device_left.NewData = true;
                            SetState(States.None, 2f);
                        });

                        BluetoothLEHardwareInterface.SubscribeCharacteristic(device_right.Address, FullUUID(ServiceUUID), FullUUID(SubscribeCharacteristic), null, (characteristic, bytes) =>
                        {
                            //string s = ASCIIEncoding.UTF8.GetString(bytes);
                            //right_foot_data.text = s;
                            device_right.DataBytes = ASCIIEncoding.UTF8.GetString(bytes);
                            device_right.State = DEVICE_STATE.SUBSCRIBED;
                            device_right.NewData = true;
                            SetState(States.None, 2f);
                        });

                        _connected = true;

                        break;

                    case States.Unsubscribe:
                        BluetoothLEHardwareInterface.UnSubscribeCharacteristic(_deviceAddress, ServiceUUID, SubscribeCharacteristic, null);
                        SetState(States.Disconnect, 4f);
                        break;

                    case States.Disconnect:
                        if (_connected)
                        {
                            BluetoothLEHardwareInterface.DisconnectPeripheral(_deviceAddress, (address) =>
                            {
                                BluetoothLEHardwareInterface.DeInitialize(() =>
                                {

                                    _connected = false;
                                    _state = States.None;
                                });
                            });
                        }
                        else
                        {
                            BluetoothLEHardwareInterface.DeInitialize(() =>
                            {
                                _state = States.None;
                            });
                        }
                        break;
                }
            }
        }
    }

    private bool ledON = false;

    public void start_button()
    {
        SendByte((byte)0x31);
    }

    string FullUUID(string uuid)
    {
        return "6E40" + uuid + "-B5A3-F393-E0A9-E50E24DCCA9E";
    }

    bool IsEqual(string uuid1, string uuid2)
    {
        if (uuid1.Length == 4)
            uuid1 = FullUUID(uuid1);
        if (uuid2.Length == 4)
            uuid2 = FullUUID(uuid2);

        return (uuid1.ToUpper().CompareTo(uuid2.ToUpper()) == 0);
    }

    public void SendByte(byte value)
    {
        byte[] data = new byte[] { value };
        BluetoothLEHardwareInterface.WriteCharacteristic(device_left.Address, FullUUID(ServiceUUID), FullUUID(WriteCharacteristic), data, data.Length, true, (characteristicUUID) => {

        });

        BluetoothLEHardwareInterface.WriteCharacteristic(device_right.Address, FullUUID(ServiceUUID), FullUUID(WriteCharacteristic), data, data.Length, true, (characteristicUUID) => {

        });

        if (value == '1')
        {
            device_left.Initialized = true;
            device_right.Initialized = true;
        }
    }

    public void SendByteL(byte value)
    {
        byte[] data = new byte[] { value };
        BluetoothLEHardwareInterface.WriteCharacteristic(device_left.Address, FullUUID(ServiceUUID), FullUUID(WriteCharacteristic), data, data.Length, true, (characteristicUUID) => {

        });
    }

    public void SendByteR(byte value)
    {
        byte[] data = new byte[] { value };

        BluetoothLEHardwareInterface.WriteCharacteristic(device_right.Address, FullUUID(ServiceUUID), FullUUID(WriteCharacteristic), data, data.Length, true, (characteristicUUID) => {

        });
    }

    public void SendBytes(byte[] data)
    {
        BluetoothLEHardwareInterface.WriteCharacteristic(device_left.Address, FullUUID(ServiceUUID), FullUUID(WriteCharacteristic), data, data.Length, true, (characteristicUUID) => {

        });

        BluetoothLEHardwareInterface.WriteCharacteristic(device_right.Address, FullUUID(ServiceUUID), FullUUID(WriteCharacteristic), data, data.Length, true, (characteristicUUID) => {

        });
    }
}
