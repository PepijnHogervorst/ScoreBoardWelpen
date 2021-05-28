using MQTTnet;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfScoreboard.Classes
{
    public class MqttBroker
    {
        #region Event raising
        #endregion


        #region Private properties
        private IMqttServer _broker = null;
        #endregion


        #region Public properties        
        #endregion


        #region Constructor
        public MqttBroker()
        {

        }

        ~MqttBroker()
        {
            Stop();
        }
        #endregion


        #region Public methods
        public async Task Start()
        {
            if (_broker != null) return;

            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpointPort(1883);

            // In try catch for if 
            try
            {
                // Create broker and start it
                _broker = new MqttFactory().CreateMqttServer();
                await _broker.StartAsync(optionsBuilder.Build());
            }
            catch (Exception)
            {

            }
            
        }

        public async Task Stop()
        {
            if (_broker == null) return;

            try
            {
                await _broker.StopAsync();
                _broker.Dispose();
                _broker = null;
            }
            catch (Exception)
            {
            }
            
        }
        #endregion


        #region Private methods
        #endregion
    }
}
