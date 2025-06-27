using MaterialDesignColors.Recommended;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace WpfScoreboard.Classes
{
    
    public class MQTTClient
    {
        #region Event raising
        public event EventHandler OnReadyReceived;
        #endregion

        #region Private properties
        private MqttClient _client;
        private static readonly string[] topics = new string[]
        {
            "Scoreboard/Status",
            "Scoreboard/Ready",
        };

        private static readonly string[] topics_write = new string[]
        {
            "Scoreboard/ButtonProgram",
            "Scoreboard/Party",
            "Scoreboard/Brightness",
            "Scoreboard/Clear",
            "Scoreboard/Strobe",
        };
        #endregion

        #region Public properties
        public int Brightness { get; set; }
        public string Status { get; set; }
        public string ArduinoVersion { get; set; }
        #endregion

        #region Constructor
        public MQTTClient()
        {
            _client = new MqttClient("127.0.0.1");

            _client.MqttMsgPublishReceived += _client_MqttMsgPublishReceived;
            _client.ConnectionClosed += _client_ConnectionClosed;

            Brightness = -1;
        }
        #endregion

        #region Event methods
        private void _client_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            if (e.Topic.Equals(topics[0]))
            {
                GetStatusProperties(e.Message);
            }
            else if (e.Topic.Equals(topics[1]))
            {
                // Send feedback to UI
                OnReadyReceived?.Invoke(this, new EventArgs());
            }


            
        }
        private void _client_ConnectionClosed(object sender, EventArgs e)
        {
            
        }
        #endregion

        #region Public methods
        public void Connect()
        {
            if (_client == null)
            {
                _client = new MqttClient("127.0.0.1");

                _client.MqttMsgPublishReceived += _client_MqttMsgPublishReceived;
                _client.ConnectionClosed += _client_ConnectionClosed;
            }
            _client.Connect("ScoreboardApp_" + Guid.NewGuid().ToString());

            if (_client.IsConnected)
            {
                var qosLevels = new byte[topics.Length];
                for (int i = 0; i < qosLevels.Length; i++)
                {
                    qosLevels[i] = MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE;
                }
                _client.Subscribe(topics, qosLevels);
            }
        }

        public void Close()
        {
            if (_client.IsConnected)
            {
                _client.Disconnect();
                _client.MqttMsgPublishReceived -= _client_MqttMsgPublishReceived;
                _client.ConnectionClosed -= _client_ConnectionClosed;
                _client = null;
            }
        }

        public void SetScore(int group, int points)
        {
            var data = new ScoreboardData
            {
                Group = group,
                Points = points,
            };

            Publish(topics_write[0], JsonConvert.SerializeObject(data));
        }

        public void SetParty(int program, int delay)
        {
            var data = new PartyData
            {
                Program = program,
                Delay = delay,
            };

            Publish(topics_write[1], JsonConvert.SerializeObject(data));
        }

        public void SetBrightness(int brightness)
        {
            var data = new BrightnessData
            {
                Brightness = brightness,
            };

            Publish(topics_write[2], JsonConvert.SerializeObject(data));
        }

        public void ClearLEDs()
        {
            var data = new ClearLEDsData();
            Publish(topics_write[3], JsonConvert.SerializeObject(data));
        }

        public void SetStrobe(bool active, int time, int red, int green, int blue, int brightness)
        {
            // 0xGGRRBB
            int color = (green << 16) + (red << 8) + blue;
            var data = new StrobeData
            {
                Active = active,
                Time = time,
                Color = color,
                Brightness = brightness,
            };

            Publish(topics_write[4], JsonConvert.SerializeObject(data));
        }
        #endregion

        #region Private methods
        private void Publish(string topic, string message)
        {
            if (_client == null) return;
            if (!_client.IsConnected) return;

            _client.Publish(topic, Encoding.UTF8.GetBytes(message));
        }

        private void GetStatusProperties(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);

            try
            {
                var status = JsonConvert.DeserializeObject<StatusData>(json);

                Status = status.State;
                Brightness = status.Brightness;
                ArduinoVersion = status.Version;
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region JSON parsing classes
        private class ScoreboardData
        {
            public int Group { get; set; }
            public int Points { get; set; }
        }

        private class PartyData
        {
            public int Program { get; set; }
            public int Delay { get; set; }
        }

        private class BrightnessData
        {
            public int Brightness { get; set; }
        }

        private class ClearLEDsData
        {
            public bool Clear { get; set; }
        }

        private class StatusData
        {
            public string State { get; set; }
            public int Brightness { get; set; }
            public string Version { get; set; }
        }

        private class StrobeData
        {
            public bool Active { get; set; }
            public int Brightness { get; set; }
            public int Time { get; set; }
            public int Color { get; set; }
        }
        #endregion
    }


}
