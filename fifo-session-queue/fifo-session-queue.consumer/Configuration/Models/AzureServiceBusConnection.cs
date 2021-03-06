﻿using Microsoft.Azure.ServiceBus;

namespace fifo_session_queue.consumer.Configuration.Models
{
    public class AzureServiceBusConnection
    {
        public string Endpoint { get; set; }
        public string SasKey { get; set; }
        public string SasKeyName { get; set; }
        public TransportType TransportType { get; set; }
    }
}
