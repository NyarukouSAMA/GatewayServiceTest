using MediatR;
using MSP.QueueClient.Abstractions;
using MSP.QueueClient.Extensions;
using MSP.QueueClient.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MSP.QueueClient
{
    public class RabbitMQConsumer<TData> : IRabbitMQConsumer, IDisposable
    {
        private readonly string _queueName;

        IRabbitMQConnectionProvider _connectionProvider;
        IMediator _mediator;

        IModel _channel;
        EventingBasicConsumer _consumer;

        private bool _isConsuming = false;
        private bool _isProcessStopping = false;
        private int _activityCounter = 0;

        public RabbitMQConsumer(string queueName,
            IRabbitMQConnectionProvider connectionProvider,
            IMediator mediator)
        {
            _queueName = queueName;
            _connectionProvider = connectionProvider;
            _mediator = mediator;
        }

        public Action<Exception> ErrorHandler;

        public void StartConsuming()
        {
            if (_isConsuming)
            {
                return;
            }

            _channel = _connectionProvider.CreateChannelFromHostedConnection();
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += async (model, ea) =>
            {
                if (!_isProcessStopping)
                {
                    _activityCounter++;
                    try
                    {
                        var message = new QueueMessage<TData>()
                        {
                            Headers = ea.BasicProperties?.Headers,
                            Data = JsonConvert.DeserializeObject<TData>(Encoding.UTF8.GetString(ea.Body.ToArray())),
                            RoutingKey = ea.RoutingKey,
                            CorrelationId = ea.BasicProperties?.CorrelationId,
                            MessageId = ea.BasicProperties?.MessageId,
                            XDeath = ea.GetXDeaths()
                        };

                        bool handlerResult = await _mediator.Send(message);

                        if (handlerResult)
                        {
                            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        }
                        else
                        {
                            _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler?.Invoke(ex);
                        _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    }
                    finally
                    {
                        _activityCounter--;
                    }

                }
            };

            _channel.BasicConsume(queue: _queueName,
                autoAck: false,
                consumer: _consumer);

            _isConsuming = true;
        }

        public void StopConsuming()
        {
            _isProcessStopping = true;

            var replayCount = 0;
            while (_activityCounter > 0 && replayCount < 10)
            {
                Task.Delay(1000).Wait();
                replayCount++;
            }

            if (_consumer != null)
            {
                _consumer.OnCancel();
                _consumer = null;
            }

            if (_channel != null)
            {
                _channel.Close();
                _channel.Dispose();
                _channel = null;
            }

            _isConsuming = false;
            _isProcessStopping = false;
        }

        public void Dispose()
        {
            StopConsuming();
        }
    }
}
