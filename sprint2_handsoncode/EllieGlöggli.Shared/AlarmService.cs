using Grpc.Core;

namespace EllieGlöggli.Common
{
    public class AlarmService
    {
        public delegate void StateChangedHandler(object sender, ResponseEventArgs e);
        public event StateChangedHandler? StateChanged;

        private volatile CancellationTokenSource tokenSource = new();

        private readonly EllieGloeggeli.EllieGloeggeliClient ellieGlöggli;

        public AlarmService(EllieGloeggeli.EllieGloeggeliClient ellieGlöggli)
        {
            this.ellieGlöggli = ellieGlöggli;
        }
            
        public async Task Subscribe(RegisterRequest registerRequest)
        {
            if (tokenSource.IsCancellationRequested)
            {
                return;
            }

            WriteResponse($"Subscription sent");
            var streamingCall = ellieGlöggli.SubscribeForGloeggli(registerRequest);

            try
            {
                var items = streamingCall.ResponseStream.ReadAllAsync(tokenSource.Token).ConfigureAwait(false);
                await foreach (var item in items)
                {
                    WriteResponse($"Notification Recieved from {item.SenderClientId} ({item.Name})");
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

        public async Task Unsubscribe(RegisterRequest registerRequest)
        {
            tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();

            await ellieGlöggli.UnsubscribeForGloeggliAsync(registerRequest);
            WriteResponse($"Unsubscription sent");
        }

        public async Task FireAlarmAsync(GloeggeliRequest gloeggeliRequest)
        {
            WriteResponse($"Notification sent");

            await ellieGlöggli.PublishGloeggeliAsync(gloeggeliRequest);
        }

        public async Task<IReadOnlyCollection<RegisterRequest>> GetRegisteredAsync()
        {
            var list = await ellieGlöggli.GetRegisteredAsync(new EmptyMessage());
            return list.Registered;
        }

        private void WriteResponse(string response)
        {
            StateChanged?.Invoke(this, new ResponseEventArgs { Response = response });
        }
    }
}