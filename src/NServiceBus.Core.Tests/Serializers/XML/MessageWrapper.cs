namespace NServiceBus.Core.Tests.Serializers.XML
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using MessageInterfaces.MessageMapper.Reflection;
    using NUnit.Framework;
    using Settings;
    using Unicast.Messages;

    [Serializable]
    public class MessageWrapper : IMessage
    {
        public string IdForCorrelation { get; set; }

        public string Id { get; set; }

        public MessageIntentEnum MessageIntent { get; set; }

        public string ReplyToAddress { get; set; }

        public TimeSpan TimeToBeReceived { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public byte[] Body { get; set; }

        public string CorrelationId { get; set; }

        public bool Recoverable { get; set; }
    }

    public class When_message_is_registered_in_conventions_but_not_in_metadata_registry
    {
        [Test]
        public void XmlSerializer_do_not_deserialize_properties()
        {
            var settings = new SettingsHolder();
            var conventions = settings.GetOrCreate<Conventions>();
            conventions.AddSystemMessagesConventions(t => t == typeof(MessageWrapper));
            var metadataRegistry = new MessageMetadataRegistry(conventions);
            //metadataRegistry.GetMessageMetadata(typeof(MessageWrapper));

            settings.Set<MessageMetadataRegistry>(metadataRegistry);
            var serializer = new XmlSerializer().Configure(settings);


            var mapper = new MessageMapper();
            mapper.Initialize(new[]
            {
                typeof(MessageWrapper)
            });

            var bytes = Encoding.UTF8.GetBytes(
                "<?xml version=\"1.0\"?><MessageWrapper xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://tempuri.net/NServiceBus.Azure.Transports.WindowsAzureStorageQueues\"><Id>1d452c1e-4011-4817-bd3c-a66000dcac53</Id><MessageIntent>Subscribe</MessageIntent><ReplyToAddress>MultipleVersionsOfAMessageIsPublished.V2Subscriber@DefaultEndpointsProtocol=https;AccountName=szymonkulecparticular;AccountKey=Ze/galSVvB7Nw0jKc4Cjioph2qK+1C6ddcOxTANYqkpzMpViWIOe9CimHYDBXfANiz+mz7YQ0nph7OU2yN0zRg==</ReplyToAddress><TimeToBeReceived>P10675199DT2H48M5.4775807S</TimeToBeReceived><Headers><NServiceBus.KeyValuePairOfStringAndString><Key>NServiceBus.ControlMessage</Key><Value>True</Value></NServiceBus.KeyValuePairOfStringAndString><NServiceBus.KeyValuePairOfStringAndString><Key>NServiceBus.MessageIntent</Key><Value>Subscribe</Value></NServiceBus.KeyValuePairOfStringAndString><NServiceBus.KeyValuePairOfStringAndString><Key>SubscriptionMessageType</Key><Value>NServiceBus.AcceptanceTests.Versioning.When_multiple_versions_of_a_message_is_published+V2Event, NServiceBus.Azure.Transports.WindowsAzureStorageQueues.AcceptanceTests, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null</Value></NServiceBus.KeyValuePairOfStringAndString><NServiceBus.KeyValuePairOfStringAndString><Key>NServiceBus.ReplyToAddress</Key><Value>MultipleVersionsOfAMessageIsPublished.V2Subscriber@DefaultEndpointsProtocol=https;AccountName=szymonkulecparticular;AccountKey=Ze/galSVvB7Nw0jKc4Cjioph2qK+1C6ddcOxTANYqkpzMpViWIOe9CimHYDBXfANiz+mz7YQ0nph7OU2yN0zRg==</Value></NServiceBus.KeyValuePairOfStringAndString><NServiceBus.KeyValuePairOfStringAndString><Key>NServiceBus.SubscriberAddress</Key><Value>MultipleVersionsOfAMessageIsPublished.V2Subscriber</Value></NServiceBus.KeyValuePairOfStringAndString><NServiceBus.KeyValuePairOfStringAndString><Key>NServiceBus.SubscriberEndpoint</Key><Value>MultipleVersionsOfAMessageIsPublished.V2Subscriber</Value></NServiceBus.KeyValuePairOfStringAndString><NServiceBus.KeyValuePairOfStringAndString><Key>NServiceBus.TimeSent</Key><Value>2016-08-12 11:23:26:779770 Z</Value></NServiceBus.KeyValuePairOfStringAndString><NServiceBus.KeyValuePairOfStringAndString><Key>NServiceBus.Version</Key><Value>6.0.0</Value></NServiceBus.KeyValuePairOfStringAndString></Headers><Body></Body><Recoverable>true</Recoverable></MessageWrapper>");

            var s = serializer(mapper);
            var objects = s.Deserialize(new MemoryStream(bytes), new List<Type>
            {
                typeof(MessageWrapper)
            });
            var deserialized = objects.OfType<MessageWrapper>().SingleOrDefault();

            Assert.NotNull(deserialized.Headers);
        }
    }
}