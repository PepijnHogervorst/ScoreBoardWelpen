using WelpenScoreboard.Application.Mqtt;
using WelpenScoreboard.Application.Mqtt.Events;
using WelpenScoreboard.Application.Mqtt.Workers;

namespace WelpenScoreboard.Infrastructure.Mqtt;
internal class MqttControl : IMqttControl
{
    private readonly IMqttBroker _broker;
    private readonly IMqttClient _client;
    private readonly IMqttAliveWorker _aliveWorker;
    private readonly Dictionary<string, IMqttTopicParser> _topicParsers;

    public MqttControl(IMqttBroker broker,
                       IMqttClient client,
                       IMqttAliveWorker aliveWorker,
                       IEnumerable<IMqttTopicParser> topicParsers)
    {
        _broker = broker;
        _client = client;
        _aliveWorker = aliveWorker;
        _topicParsers = topicParsers.ToDictionary(t => t.Topic);
        Topics = [.. topicParsers.Select(p => p.Topic)];
        _client.SetCallback(ProcessMessageReceived);
    }

    public List<string> Topics { get; }

    public async Task<bool> StartAsync()
    {
        await _broker.StartAsync();

        if (!await _client.ConnectAsync()) return false;
        if (!await SubscribeToTopics()) return false;
        StartWorkers();
        return true;
    }

    public async Task StopAsync()
    {
        StopWorkers();
        await _client.CloseAsync();
        await _broker.StopAsync();
    }

    private async Task ProcessMessageReceived(MqttMessageReceivedEventArgs args)
    {
        if (!_topicParsers.TryGetValue(args.Topic, out var parser)) return;

        await parser.ParseAsync(args.Message);
    }

    private void StartWorkers()
    {
        _aliveWorker.Start();
    }

    private void StopWorkers()
    {
        _aliveWorker.Stop();
    }

    private async Task<bool> SubscribeToTopics()
    {
        bool result = true;
        foreach (string topic in Topics)
        {
            if (await _client.SubscribeAsync(topic)) continue;
            result = false;
        }
        return result;
    }
}
