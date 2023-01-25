using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace OB_Net_Core_Tutorial.ListenerProject
{
    public class MessageListener : IHostedService, IDisposable
    {
        private readonly EventProcessorClient processor;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        public MessageListener(IConfiguration config, ILogger<MessageListener> logger)
        {
            _logger = logger;
            _config = config;

            string topic = "onboardingtopic";
            string azureContainername = "carstorage";
            string eventHubConn = "Endpoint=sb://evhonboarding.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=5mITZKm/Wo8b2SA53UewZxMrCoQ+ZPmVeEFDawzdYcA=";
            string azStorageConn = "DefaultEndpointsProtocol=https;AccountName=strsulthan;AccountKey=VIQrZSG3yT9/ceVP18/vP9XW+65o1XSsVFSqdfK8kiaB1++YUCcWbGsy6rbL+qipr0kMzlQN1aVT+ASt8KBaaw==;EndpointSuffix=core.windows.net";
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            BlobContainerClient storageClient = new BlobContainerClient(azStorageConn, azureContainername);

            processor = new EventProcessorClient(storageClient, consumerGroup, eventHubConn, topic);

            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;
        }

        public async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            /* handle logic consume data here*/
            _logger.LogInformation(Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));
            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
        }

        public async Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            /* handle error here*/
            _logger.LogError(eventArgs.Exception.Message);
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await processor.StartProcessingAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await processor.StopProcessingAsync();
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MessageListernerService()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
