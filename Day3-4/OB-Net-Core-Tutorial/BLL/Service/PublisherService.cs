using OB_Net_Core_Tutorial.BLL.Interface;
using Newtonsoft.Json;
using OB_Net_Core_Tutorial.BLL.DTO;
using OB_Net_Core_Tutorial.DAL.Models;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace OB_Net_Core_Tutorial.BLL.Service
{
    public class PublisherService : IPublisherService
    {
        private IConfiguration _config;
        public PublisherService(IConfiguration configuration) 
        {
            _config = configuration;
        }


        public async Task SendCarToEventHub(Car car)
        {
            string connString = _config.GetValue<string>("EventHub:ConnectionString");
            string topic = _config.GetValue<string>("EventHub:EventHubNameTest");

            //create event hub producer
            await using var publisher = new EventHubProducerClient(connString, topic);

            //create batch
            using var eventBatch = await publisher.CreateBatchAsync();

            //add message, ini bisa banyak sekaligus
            var message = JsonConvert.SerializeObject(car);
            eventBatch.TryAdd(new EventData(new BinaryData(message)));

            //send message
            await publisher.SendAsync(eventBatch);
        }
    }
}
