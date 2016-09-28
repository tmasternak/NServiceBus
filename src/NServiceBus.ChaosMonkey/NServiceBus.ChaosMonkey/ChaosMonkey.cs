namespace NServiceBus.ChaosMonkey
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Extensibility;
    using Features;
    using Pipeline;
    using Routing;
    using Transport;

    public class ChaosMonkey : Feature
    {
        protected override void Setup(FeatureConfigurationContext context)
        {
            context.Pipeline.Register(
                b => new ChaosBehavior(b.Build<IDispatchMessages>(), context.Settings.LocalAddress()),
                "Simulates message processing anomalies.");
        }

        class ChaosBehavior : Behavior<ITransportReceiveContext>
        {
            public ChaosBehavior(IDispatchMessages messageDispatcher, string localAddress)
            {
                this.messageDispatcher = messageDispatcher;
                this.localAddress = localAddress;
            }

            public override async Task Invoke(ITransportReceiveContext context, Func<Task> next)
            {
                Console.WriteLine("Message seen in behavior");

                if (context.Message.Headers.ContainsKey(ChaosMonkeyHeader) == false)
                {
                    var clone = CreateClone(context.Message);

                    await SendLocal(clone);
                }

                await next().ConfigureAwait(false);
            }

            async Task SendLocal(OutgoingMessage message)
            {
                var operations = new TransportOperations(new TransportOperation(
                    message,
                    new UnicastAddressTag(localAddress),
                    DispatchConsistency.Isolated));

                await messageDispatcher.Dispatch(operations, new TransportTransaction(), new ContextBag());
            }

            OutgoingMessage CreateClone(IncomingMessage incomingMessage)
            {
                var physicalId = Guid.NewGuid().ToString();

                var headers = incomingMessage.Headers.ToDictionary(kv => kv.Key, kv => kv.Value);
                headers.Add(ChaosMonkeyHeader, string.Empty);

                var outgoingMessage = new OutgoingMessage(physicalId, headers, incomingMessage.Body);

                return outgoingMessage;
            }

            string localAddress;
            IDispatchMessages messageDispatcher;

            const string ChaosMonkeyHeader = "ChaosMonkey";
        }
    }
}