using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

public class Level2Script : MonoBehaviour
{
	public List<Text> Buttons;
    public List<Text> ReceivedData;
    public List<bool> Connected;
    public List<string> Services;
	public List<string> Characteristics;
    public Text device_uuid;
    public string DeviceName = "SmartInsole";
    public string ServiceUUID = "0001";
    public string SubscribeCharacteristic = "0003";
    public string WriteCharacteristic = "0002";
    private bool _connected = false;
    private float _timeout = 0f;
    private string _deviceAddress;
    private bool _foundSubscribeID = false;
    private bool _foundWriteID = false;
    private byte[] _dataBytes = null;
    private bool _rssiOnly = false;
    private int _rssi = 0;
    public string add1 = "C8:8D:80:48:EA:7A";
    public string add2 = "D4:7A:BC:EC:3F:AA";
    public int num_devices = 0;
    int flag = 1;

    private void Start()
    {
        BluetoothLEHardwareInterface.Initialize(true, false, () => {

            FoundDeviceListScript.DeviceAddressList = new List<DeviceObject>();

            BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) => {

                if (name == "SmartInsole")
                {
                    FoundDeviceListScript.DeviceAddressList.Add(new DeviceObject(address, name));
                    num_devices++;
                }

            }, null);

        }, (error) => {

            BluetoothLEHardwareInterface.Log("BLE Error: " + error);

        });
    }

    private void Update()
    {
        if(num_devices == 2 && flag == 1)
        {
            OnButtonClick(0);
            OnSubscribeClick(0);
            OnButtonClick(1);
            OnSubscribeClick(1);
            flag = 0;
        }
    }

    public void OnScanClick()
    {
        num_devices = 0;
        FoundDeviceListScript.DeviceAddressList = new List<DeviceObject>();
        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) => {

            if (name == "SmartInsole")
            {
                FoundDeviceListScript.DeviceAddressList.Add(new DeviceObject(address, name));
                num_devices++;
            }

        }, null);
    }

    // Use this for initialization
    public void displayList()
	{
        int buttonID = 0;
        foreach (var device in FoundDeviceListScript.DeviceAddressList)
        {
            Buttons[buttonID++].text = device.Name;
            if (buttonID == 4)
                break;
        }
    }

    void OnCharacteristic (string characteristic, byte[] bytes)
	{
        string s = ASCIIEncoding.UTF8.GetString(bytes);
        int ID = s[0];
        ID -= 49;
        Text button = ReceivedData[ID];
        button.text = s;
        device_uuid.text = s + " " + s[0];
        BluetoothLEHardwareInterface.Log ("received: " + characteristic);
	}

	public void OnSubscribeClick (int buttonID)
	{
		if (buttonID >= 0 && buttonID < 4)
		{
			DeviceObject device = FoundDeviceListScript.DeviceAddressList[buttonID];
            string subscribedService = Services[buttonID];
			string subscribedCharacteristic = Characteristics[buttonID];

			if (Connected[buttonID])
			{
				BluetoothLEHardwareInterface.Log ("subscribing to: " + subscribedService + ", " + subscribedCharacteristic);

				BluetoothLEHardwareInterface.SubscribeCharacteristic (device.Address, subscribedService, subscribedCharacteristic, null, (characteristic, bytes) => 
                {
                    string s = ASCIIEncoding.UTF8.GetString(bytes);
                    //int ID = s[0];
                    //ID -= 49;
                    Text button = ReceivedData[buttonID];
                    button.text = s;
                    BluetoothLEHardwareInterface.Log ("received data: " + characteristic);
				});
			}
		}
	}

	public void OnButtonClick (int buttonID)
	{
		if (buttonID >= 0 && buttonID < 4)
		{
			DeviceObject device = FoundDeviceListScript.DeviceAddressList[buttonID];
			Text button = Buttons[buttonID];
			string subscribedService = Services[buttonID];
			string subscribedCharacteristic = Characteristics[buttonID];

			if (device != null && button != null)
			{
				if (Connected[buttonID])
				{
					/*if (_foundSubscribeID && _foundWriteID)
					{
						BluetoothLEHardwareInterface.UnSubscribeCharacteristic (device.Address, subscribedService, subscribedCharacteristic, (characteristic) => {
							
							Services[buttonID] = null;
							Characteristics[buttonID] = null;
							
							BluetoothLEHardwareInterface.DisconnectPeripheral (device.Address, (disconnectAddress) => 
                            {
                                BluetoothLEHardwareInterface.DeInitialize(() =>
                                {
                                    button.text = device.Name;
                                    _connected = false;
                                    _foundSubscribeID = false;
                                    _foundWriteID = false;
                                });
							});
						});
					}
					else
					{*/
						BluetoothLEHardwareInterface.DisconnectPeripheral (device.Address, (disconnectAddress) => 
                        {
							button.text = device.Name;
                            Connected[buttonID] = false;
                        });
					//}
				}
				else
				{
					BluetoothLEHardwareInterface.ConnectToPeripheral (device.Address, (address) => {

					}, null, (address, service, characteristic) => {

                        if (IsEqual(service, ServiceUUID))
                        {
                            _foundSubscribeID = _foundSubscribeID || IsEqual(characteristic, SubscribeCharacteristic);
                            _foundWriteID = _foundWriteID || IsEqual(characteristic, WriteCharacteristic);

                            // if we have found both characteristics that we are waiting for
                            // set the state. make sure there is enough timeout that if the
                            // device is still enumerating other characteristics it finishes
                            // before we try to subscribe
                            if (_foundSubscribeID && _foundWriteID)
                            {
                                if (string.IsNullOrEmpty(Services[buttonID]) && string.IsNullOrEmpty(Characteristics[buttonID]))
                                {
                                    _foundSubscribeID = false;
                                    _foundWriteID = false;
                                    Services[buttonID] = FullUUID(service);
                                    Characteristics[buttonID] = FullUUID(characteristic);
                                    device_uuid.text = Services[buttonID] + "\n" + Characteristics[buttonID];
                                    button.text = device.Name + " connected";
                                    Connected[buttonID] = true;
                                }
                            }
                        }
					}, null);
				}
			}
		}
	}
	
	string FullUUID (string uuid)
	{
		if (uuid.Length == 4)
            return "6E40" + uuid + "-B5A3-F393-E0A9-E50E24DCCA9E";
        //return "0000" + uuid + "-0000-1000-8000-00805f9b34fb";

        return uuid;
	}

    bool IsEqual(string uuid1, string uuid2)
    {
        if (uuid1.Length == 4)
            uuid1 = FullUUID(uuid1);
        if (uuid2.Length == 4)
            uuid2 = FullUUID(uuid2);

        return (uuid1.ToUpper().CompareTo(uuid2.ToUpper()) == 0);
    }
}
