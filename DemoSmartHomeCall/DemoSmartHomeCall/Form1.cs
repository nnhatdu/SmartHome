using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace DemoSmartHomeCall
{
    public partial class Form1 : Form
    {
        private const string MQTTServer = "broker.mqtt-dashboard.com";
        private const string Topic = "/demo_project3";
        private static readonly Dictionary<string, MqttClient> QueryConnection = new();
        private static bool _isRunning = true;
        private readonly Dictionary<string, MqttClient> _deviceConnection = new();
        private readonly Dictionary<string, string> account = new(){{"admin","admin"}};
        private readonly List<string> _deviceName = new()
            {"door_lock", "alarm_system", "home_led", "air_conditioner", "water_heater"};

        private readonly CustomClass _myPropertyGrid = new();
        private readonly Random _random = new(Guid.NewGuid().GetHashCode());
        private readonly Thread _thread = new(UpdateUsage);

        public Form1()
        {
            InitializeComponent();
        }

        private class AnalysisInfo
        {
            public long time { get; set; }
            public int num { get; set; }

            public AnalysisInfo(long time, int num)
            {
                this.time = time;
                this.num = num;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // init default device
            lock (_deviceConnection)
            {
                deviceList.SelectedObject = _myPropertyGrid;
            }

            foreach (var device in _deviceName)
            {
                lock (_myPropertyGrid)
                {
                    _myPropertyGrid.Add(new CustomProperty(device, DeviceState.Off,
                        typeof(DeviceState), false, true));
                }

                _deviceConnection.Add(device, new MqttClient(MQTTServer));
                _deviceConnection[device].Connect($"{device}_{Guid.NewGuid()}");
                _deviceConnection[device].Subscribe(new[] {$"{Topic}/manage/{device}"},
                    new[] {MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE});
                _deviceConnection[device].MqttMsgPublishReceived += (_, args) =>
                {
                    lock (_deviceConnection)
                    lock (_myPropertyGrid)
                    {
                        var msg = Encoding.UTF8.GetString(args.Message);
                        if (!msg.Equals("2") && !msg.Equals("3")) return;
                        var index = 0;
                        for (; _myPropertyGrid[index].Name != device; index++)
                        {
                        }
                        _myPropertyGrid[index].Value = msg.Equals("3") ? DeviceState.On : DeviceState.Off;
                        deviceList.Refresh();
                    }
                };
            }

            #region OnConnect

            lock (QueryConnection)
            {
                QueryConnection.Add("connect", new MqttClient(MQTTServer));
                var connect = QueryConnection["connect"];
                connect.Connect("connect_" + Guid.NewGuid());
                connect.Subscribe(new[] {$"{Topic}/connect"},
                    new[] {MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE});
                connect.MqttMsgPublishReceived += (_, _) =>
                {
                    foreach (var device in _deviceName)
                    {
                        var index = 0;
                        for (; _myPropertyGrid[index].Name != device; index++)
                        {
                        }

                        var topic = $"{Topic}/manage/{device}";
                        connect.Publish(topic,
                            Encoding.UTF8.GetBytes((DeviceState) _myPropertyGrid[index].Value == DeviceState.Off
                                ? "0"
                                : "1"),
                            MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                    }
                };
            }

            #endregion

            #region Analysis

            lock (QueryConnection)
            {
                QueryConnection.Add("analysis", new MqttClient(MQTTServer));
                var analysis = QueryConnection["analysis"];
                analysis.Connect("analysis_" + Guid.NewGuid());
                analysis.Subscribe(new[] {$"{Topic}/analysis"},
                    new[] {MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE});
                analysis.MqttMsgPublishReceived += (_, _) =>
                {
                    var now = Convert.ToInt32(DateTime.Now.ToString("HH"));
                    Parallel.ForEach(_deviceName, (device, _, _) =>
                    {
                        var date = DateTimeOffset.Now.AddDays(-6);
                        var topic = $"{Topic}/analysis/{device}";
                        var arr = new AnalysisInfo[7];
                        for (var i = 0; i < 6; i++, date = date.AddDays(1))
                        {
                            arr[i] = new AnalysisInfo(date.ToUnixTimeMilliseconds(), 
                                _random.Next(4, 24));
                        }
                        arr[6] = new AnalysisInfo(date.ToUnixTimeMilliseconds(), _random.Next(0, now));
                        var msg = JsonSerializer.Serialize(arr);
                        analysis.Publish(topic, Encoding.UTF8.GetBytes(msg));
                    });
                };
            }

            #endregion

            #region Monitor

            lock (QueryConnection)
            {
                QueryConnection.Add("ram_analysis", new MqttClient(MQTTServer));
                QueryConnection.Add("cpu_analysis", new MqttClient(MQTTServer));
            }

            
            
            #endregion

            #region Login
            lock (QueryConnection)
            {
                QueryConnection.Add("login", new MqttClient(MQTTServer));
                var login = QueryConnection["login"];
                login.Connect($"login_{Guid.NewGuid()}");
                login.Subscribe(new[] {$"{Topic}/login"},
                    new[] {MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE});
                login.MqttMsgPublishReceived += (_, msg) =>
                {
                    var message = Encoding.UTF8.GetString(msg.Message).Split('|');
                    if (message.Length != 2)
                        return;
                    string result;
                    if (!account.ContainsKey(message[0]))
                        result = "1";
                    else if (account[message[0]].Equals(message[1]))
                        result = "0";
                    else result = "2";
                    login.Publish($"{Topic}/login", Encoding.UTF8.GetBytes(result));
            };
            }

            #endregion
            _thread.Start();
            deviceList.Refresh();
        }

        private static void UpdateUsage()
        {
            MqttClient ramAnalysis;
            MqttClient cpuAnalysis;
            lock (QueryConnection)
            {
                ramAnalysis = QueryConnection["ram_analysis"];
                cpuAnalysis = QueryConnection["cpu_analysis"];
            }

            ramAnalysis.Connect("ram_monitor_" + Guid.NewGuid());
            cpuAnalysis.Connect("cpu_monitor_" + Guid.NewGuid());
            var ramTopic = $"{Topic}/monitor/ram";
            var cpuTopic = $"{Topic}/monitor/cpu";
            var cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            var ram = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            while (_isRunning)
            {
                ramAnalysis.Publish(ramTopic,
                    Encoding.UTF8.GetBytes(Convert.ToString(ram.NextValue(), CultureInfo.InvariantCulture)));
                cpuAnalysis.Publish(cpuTopic,
                    Encoding.UTF8.GetBytes(Convert.ToString(cpu.NextValue(), CultureInfo.InvariantCulture)));
                Thread.Sleep(5000);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _isRunning = false;
            lock (_deviceConnection)
            {
                foreach (var (_, device) in _deviceConnection) device.Disconnect();
            }

            lock (QueryConnection)
            {
                foreach (var (_, query) in QueryConnection) query.Disconnect();
            }

            deviceList.Dispose();
        }

        private void deviceList_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            var topic = $"{Topic}/manage/{e.ChangedItem.Label}";
            var oldValue = (DeviceState) e.OldValue;
            _deviceConnection[e.ChangedItem.Label!].Publish(topic,
                Encoding.UTF8.GetBytes(oldValue == DeviceState.Off ? "1" : "0"),
                MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
        }

        private enum DeviceState
        {
            On,
            Off
        }
    }
}