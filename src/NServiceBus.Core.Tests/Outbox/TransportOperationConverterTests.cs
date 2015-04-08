﻿namespace NServiceBus.Core.Tests.Outbox
{
    using System;
    using NServiceBus.Outbox;
    using NServiceBus.Unicast;
    using NUnit.Framework;

    [TestFixture]
    class TransportOperationConverterTests
    {
        [Test]
        public void SendOptions()
        {
            var options = new SendOptions("destination", DateTime.UtcNow.AddDays(1))
            {
                TimeToBeReceived = TimeSpan.FromMinutes(1),
                NonDurable = true,
                EnlistInReceiveTransaction = false,
                EnforceMessagingBestPractices = false
            };

            var converted = (SendOptions)options.ToTransportOperationOptions().ToDeliveryOptions();

            Assert.AreEqual(converted.DeliverAt.ToString(), options.DeliverAt.ToString()); //the ticks will be off
            Assert.AreEqual(converted.Destination, options.Destination);
            Assert.AreEqual(converted.TimeToBeReceived, options.TimeToBeReceived); 
            Assert.AreEqual(converted.NonDurable, options.NonDurable);
            Assert.AreEqual(converted.EnforceMessagingBestPractices, options.EnforceMessagingBestPractices);
            Assert.AreEqual(converted.EnlistInReceiveTransaction, options.EnlistInReceiveTransaction);

            options = new SendOptions("destination", delayDeliveryFor: TimeSpan.FromMinutes(1))
            {
                TimeToBeReceived = TimeSpan.FromMinutes(1),
                NonDurable = true,
                EnlistInReceiveTransaction = false,
                EnforceMessagingBestPractices = false
            };

            converted = (SendOptions)options.ToTransportOperationOptions().ToDeliveryOptions();
            Assert.AreEqual(converted.DelayDeliveryFor, options.DelayDeliveryFor);
        }

        [Test]
        public void PublishOptions()
        {

            var options = new PublishOptions(typeof(MyMessage))
            {
                TimeToBeReceived = TimeSpan.FromMinutes(1),
                NonDurable = true,
                EnforceMessagingBestPractices = false,
            };

            var converted = (PublishOptions)options.ToTransportOperationOptions().ToDeliveryOptions();


            Assert.AreEqual(typeof(MyMessage), options.EventType);
            Assert.AreEqual(converted.TimeToBeReceived, options.TimeToBeReceived);
            Assert.AreEqual(converted.NonDurable, options.NonDurable);
            Assert.AreEqual(converted.EnlistInReceiveTransaction, options.EnlistInReceiveTransaction);
            Assert.AreEqual(converted.EnforceMessagingBestPractices, options.EnforceMessagingBestPractices);
        }

        class MyMessage
        { }
    }
}
