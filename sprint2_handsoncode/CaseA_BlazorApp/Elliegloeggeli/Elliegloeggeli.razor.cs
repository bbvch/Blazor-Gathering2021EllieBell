using Grpc.Core;

namespace CaseA_BlazorApp.Elliegloeggeli
{
    public partial class Elliegloeggeli
    {
        private volatile CancellationTokenSource tokenSource = new CancellationTokenSource();
        private readonly System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        public string ServerResponse { get; set; } = string.Empty;

        public async Task Subscribe()
        {
            if (tokenSource.IsCancellationRequested)
            {
                return;
            }

            WriteResponse($"Subscription sent{Environment.NewLine}");
            var streamingCall = elliegloeggli.SubscribeForGloeggli(new RegisterRequest { ClientId = 1 });

            try
            {
                var items = streamingCall.ResponseStream.ReadAllAsync(tokenSource.Token).ConfigureAwait(false);
                await foreach (var item in items)
                {
                    WriteResponse($"Notification Recieved from {item.SenderClientId}{Environment.NewLine}");                    
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                WriteResponse($"Unsubscription successful");
            }
            catch (OperationCanceledException)
            {
                WriteResponse($"Unsubscription successful");
            }
        }
        public async Task Unsubscribe()
        {
            tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();

            await elliegloeggli.UnsubscribeForGloeggliAsync(new RegisterRequest { ClientId = 1 });
            WriteResponse($"Unsubscription sent{Environment.NewLine}");
        }
        public async Task Notify()
        {
            WriteResponse($"Notification sent{Environment.NewLine}");

            await elliegloeggli.PublishGloeggeliAsync(new GloeggeliRequest { SenderClientId = 2 });
        }

        private void WriteResponse(string response)
        {
            lock (stringBuilder)
            {
                stringBuilder.AppendLine($"{DateTime.Now.Ticks}: {response}");
                ServerResponse = stringBuilder.ToString();
            }
        }

    }
}
