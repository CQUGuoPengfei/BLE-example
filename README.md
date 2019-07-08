# BLE-example
.net framework BLE example code


Small example to work with BLE on win10 & .net framework 4.7.2.

1. Tested on win10 1803 (17134.407)
Might not work on previous versions, like:
https://social.msdn.microsoft.com/Forums/en-US/58da3fdb-a0e1-4161-8af3-778b6839f4e1/bluetooth-bluetoothledevicefromidasync-does-not-complete-on-10015063?forum=wdk&prof=required

2. CSR Harmony BLuetooth driver did not work for me. I have to unistall it and let windows install it's own drivers.

3. My setup is:

 a) basic  desktop computer, bluetooth dongle
 
 b) ble dongle like this http://wiki.seeedstudio.com/Grove-BLE_v1/ 
 I used this terminal to communicate with dongle https://www.compuphase.com/software_termite.htm
 
4. In order to work from scratch, you need to include two references in your solution:

 «Windows от Universal Windows Platform»
    C:\Program Files (x86)\Windows Kits\10\UnionMetadata\10.0.17134.0\Windows.winmd
    
    «System.Runtime.WindowsRuntime»
    C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5\System.Runtime.WindowsRuntime.dll

5. No idea if it works on .net core

6. In my case the device was not paired. I was able to find the code on SO which shows how to pair BLE devices.

7. "32feet" nuget lib does not work with BLE. Actually I did not find any lib/package to work with BLE.

8. I included everything from solution folder to github, please forgive me. 

Some help on code:

In connect part:
>if (args.Advertisement.LocalName.Contains("HMSoft"))

Here you should provide the name of your dongle to C# program. In my case it is HMSoft


> if (!service.Uuid.ToString().StartsWith("0000ffe0"))

Each device has uuids, so in order to do the job you should first look into documentation or enumerate all uuid's from device and choose your.
In my case it was "0000ffe0-0000-1000-8000-00805f9b34fb"

>if (!characteristic.Uuid.ToString().StartsWith("0000ffe1"))

Each device has several characteristics, so again look at docs or check all of them to see what is sending data to you.
In my case it was "0000ffe1-0000-1000-8000-00805f9b34fb"

In send part:

> var result = await gattCharacteristic.WriteValueAsync(writer);

You can not send more then 20 bytes at time. There will be an exception.


All other stuff is pretty clear.




