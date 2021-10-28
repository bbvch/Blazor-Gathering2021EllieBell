using CaseB_SharedLibrary;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using ProtoBuf.Grpc;

namespace CaseB_BlazorApp2.Elliegloeggeli
{
    public partial class Elliegloeggeli : IDisposable
    {
        [Inject]
        IEllieGloeggeliService elliegloeggli { get; set; } = null!;

        private volatile CancellationTokenSource? tokenSource;

        public string ServerResponse { get; set; } = string.Empty;

        public async Task Subscribe()
        {
            ServerResponse += $"{DateTime.Now.Ticks}: Subscription sent{Environment.NewLine}";

            tokenSource = new CancellationTokenSource();
            var options = new CallOptions(cancellationToken: tokenSource.Token);

            try
            {
                var messages = elliegloeggli.SubscribeForGloeggli(new RegisterRequest { ClientId = 1 }, new CallContext(options)).WithCancellation(tokenSource.Token);
                await foreach (var gloeggeliNotification in messages)
                {
                    ServerResponse += $"{DateTime.Now.Ticks}: Notification Recieved from {gloeggeliNotification.SenderId}{Environment.NewLine}";
                }
            }
            catch (RpcException exception) when (exception.StatusCode == StatusCode.Cancelled)
            {
                ServerResponse += $"{DateTime.Now.Ticks}: Stop Listening to notifications{Environment.NewLine}";
            }
            catch (RpcException exception)
            {
                ServerResponse += $"{DateTime.Now.Ticks}: Error recieving data '{exception.Message}'{Environment.NewLine}";
                Console.WriteLine(exception);
            }
            catch (OperationCanceledException)
            {
                ServerResponse += $"{DateTime.Now.Ticks}: Stop Listening to notifications{Environment.NewLine}";
            }
        }

        public Task Unsubscribe()
        {
            if (tokenSource == null)
            {
                return Task.CompletedTask;
            }


            tokenSource.Cancel();
            tokenSource = null;

            try
            {
                elliegloeggli.UnsubscribeForGloeggli(new RegisterRequest { ClientId = 1 }, new CallContext());
                ServerResponse += $"{DateTime.Now.Ticks}: Unsubscription sent{Environment.NewLine}";

            }
            catch (RpcException exception)
            {
                ServerResponse += $"{DateTime.Now.Ticks}: Error unsubscribing '{exception.Message}'{Environment.NewLine}";
                Console.WriteLine(exception);
            }


            return Task.CompletedTask;
        }

        public async Task Notify()
        {
            ServerResponse += $"{DateTime.Now.Ticks}: Notification sent{Environment.NewLine}";

            await elliegloeggli.PublishGloeggeli(new GloeggeliRequest { ClientId = 2 }, new CallContext());
        }

        public void Dispose()
        {
            Unsubscribe();
        }
    }
}
