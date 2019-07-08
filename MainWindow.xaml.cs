
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace BluetoothTest
{

    /*
     
        
    «Windows от Universal Windows Platform»
    C:\Program Files (x86)\Windows Kits\10\UnionMetadata\10.0.17134.0\Windows.winmd


    «System.Runtime.WindowsRuntime»
    C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5\System.Runtime.WindowsRuntime.dll


         */
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BluetoothLEAdvertisementWatcher BTWatch;

        string id1 = "00001800-0000-1000-8000-00805f9b34fb";
        string id2 = "00001801 -0000-1000-8000-00805f9b34fb";
        string id3 = "0000ffe0-0000-1000-8000-00805f9b34fb";
        string char1 = "0000ffe1-0000-1000-8000-00805f9b34fb";
        public MainWindow()
        {
            
            InitializeComponent();

            BTWatch = new BluetoothLEAdvertisementWatcher();          
            BTWatch.ScanningMode = BluetoothLEScanningMode.Active;
          
            BTWatch.Received += Watcher_Received;
            BTWatch.Start();           
            toSend = Encoding.ASCII.GetBytes("Hello World1 Hello World");
            bSend.Click += BSend_Click;
        }
        byte[] toSend;
        private async void BSend_Click(object sender, RoutedEventArgs e)
        {
            IBuffer writer = toSend.AsBuffer();
            try
            {
                var gattCharacteristic = foundCharacters.First();                      
                var result = await gattCharacteristic.WriteValueAsync(writer);
                if (result == GattCommunicationStatus.Success)
                {
                    
                    Dispatcher.Invoke(() =>
                    {                       
                        tb.Text += Environment.NewLine;
                        tb.Text += "message sent ok";                      
                    });
                }
                else
                {
                    tb.Text += Environment.NewLine;
                    tb.Text += "message sent bad";
                }
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80650003 || (uint)ex.HResult == 0x80070005)
            {
                tb.Text += Environment.NewLine;
                tb.Text += ex.Message;                
            }
        }       
      

        List<GattDeviceService> foundServices = new List<GattDeviceService>();
        List<GattCharacteristic> foundCharacters = new List<GattCharacteristic>();
        private async void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            
            if (args.Advertisement.LocalName.Contains("HMSoft"))
            {
               
                BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);
                GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync();
                if (result.Status == GattCommunicationStatus.Success)
                {
                    var services = result.Services;
                    foreach (var service in services)
                    {
                        if (!foundServices.Contains(service))
                        {
                            Dispatcher.Invoke(() =>
                            {
                                tb.Text += service.Uuid;
                                tb.Text += Environment.NewLine;
                                foundServices.Add(service);
                            });
                        }
                        if (!service.Uuid.ToString().StartsWith("0000ffe0"))
                        {
                            continue;
                        }
                        GattCharacteristicsResult characteristicsResult = await service.GetCharacteristicsAsync();
                        if (characteristicsResult.Status == GattCommunicationStatus.Success)
                        {
                            var characteristics = characteristicsResult.Characteristics;
                            foreach (var characteristic in characteristics)
                            {
                               
                                if (!foundCharacters.Contains(characteristic))
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        tb.Text += characteristic.Uuid;
                                        tb.Text += Environment.NewLine;
                                        foundCharacters.Add(characteristic);
                                    });
                                }

                                if (!characteristic.Uuid.ToString().StartsWith("0000ffe1"))
                                {
                                    continue;
                                }
                                GattCharacteristicProperties properties = characteristic.CharacteristicProperties;
                                if (properties.HasFlag(GattCharacteristicProperties.Indicate))
                                {
                                   
                                    GattWriteResult status = 
                                        await characteristic.WriteClientCharacteristicConfigurationDescriptorWithResultAsync(GattClientCharacteristicConfigurationDescriptorValue.Indicate);
                                    return;
                                }

                                if (properties.HasFlag(GattCharacteristicProperties.Notify))
                                {                                 

                                    GattCommunicationStatus status =
                                        await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                                         GattClientCharacteristicConfigurationDescriptorValue.Notify);

                                    if (status == GattCommunicationStatus.Success)
                                    {
                                        characteristic.ValueChanged += Characteristic_ValueChanged;
                                    }
                                }

                                if (properties.HasFlag(GattCharacteristicProperties.Read))
                                {                                    
                                    GattReadResult gattResult = await characteristic.ReadValueAsync();
                                    if (gattResult.Status == GattCommunicationStatus.Success)
                                    {
                                        var reader = DataReader.FromBuffer(gattResult.Value);
                                        byte[] input = new byte[reader.UnconsumedBufferLength];
                                        reader.ReadBytes(input);
                                        Dispatcher.Invoke(() =>
                                        {
                                            tb.Text += "connected: ";
                                            tb.Text += Encoding.ASCII.GetString(input);
                                            tb.Text += Environment.NewLine;
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            if (c++ < 10)
            {
                var reader = DataReader.FromBuffer(args.CharacteristicValue);
                byte[] input = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(input);
                Dispatcher.Invoke(() =>
                {

                    // input.ToList().ForEach(z => tb.Text += z.ToString() + " ");
                    tb.Text += Encoding.ASCII.GetString(input);
                    tb.Text += Environment.NewLine;
                    tb.ScrollToEnd();
                });
            }
            
        }
        static int c = 0;


    }
}

   


