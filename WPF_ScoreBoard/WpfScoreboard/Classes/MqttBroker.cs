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
        #endregion


        #region Public methods
        public async Task Start()
        {
            if (_broker != null) return;

            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpointPort(1883);

            _broker = new MqttFactory().CreateMqttServer();
            await _broker.StartAsync(optionsBuilder.Build());
        }

        public async Task Stop()
        {
            if (_broker == null) return;

            await _broker.StopAsync();
            _broker.Dispose();
            _broker = null;
        }
        #endregion


        #region Private methods
        #endregion
    }
}
