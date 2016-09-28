namespace NServiceBus.ChaosMonkey.Demo
{
    using System;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var configuration = new EndpointConfiguration("ChaosMonkey.Demo");
            configuration.UsePersistence<InMemoryPersistence>();
            configuration.UseTransport<MsmqTransport>();

            configuration.EnableFeature<ChaosMonkey>();

            var endpoint = await Endpoint.Start(configuration);

            await endpoint.SendLocal(new PlaceOrder());

            Console.WriteLine("Press any <key> to close ...");
            Console.ReadLine();
        }

        class OrderHandler : IHandleMessages<PlaceOrder>
        {
            public Task Handle(PlaceOrder message, IMessageHandlerContext context)
            {
                Console.WriteLine("PlaceOrder received.");

                return Task.FromResult(0);
            }
        }

        class PlaceOrder : ICommand
        {

        }
    }
}
