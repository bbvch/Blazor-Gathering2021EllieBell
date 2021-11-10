namespace EllieBell
{
    using System.Runtime.CompilerServices;
    using System.Text;
    using CaseA_GrpsService;
    using Grpc.Core;

    public class AlarmService
    {
        private const int ClientId = 2;

        private readonly EllieGloeggeli.EllieGloeggeliClient grpcClient;

        public AlarmService(EllieGloeggeli.EllieGloeggeliClient grpcClient)
        {
            this.grpcClient = grpcClient;
        }
            public string AlarmSent { get; set; } = string.Empty;

            public async Task SendAlarmAsync()
            {

                await this.grpcClient.PublishGloeggeliAsync(new GloeggeliRequest { SenderClientId = ClientId });
                AlarmSent += $"{DateTime.Now.ToShortTimeString()} Alarm sent to: 'local'{Environment.NewLine}";
                
                await Task.CompletedTask;
            }

            private StringBuilder stringbuider = new StringBuilder();
            private RegisterRequest ellieRequest;

            private string ServerResponse { get; set; } = string.Empty;
            public async Task Subscribe()
            {
                this.ellieRequest = new RegisterRequest { ClientId = 1, Name = "Ellie" };
                AsyncServerStreamingCall<GloeggliNotification> streamingCall = this.grpcClient.SubscribeForGloeggli(this.ellieRequest);

                try
                {
                    var items = streamingCall.ResponseStream.ReadAllAsync().ConfigureAwait(false);
                    await foreach (var item in items)
                    {
                        this.WriteResponse($"Notification received from {item.SenderClientId}{Environment.NewLine}");
                    }
                }
                catch (RpcException e) when(e.StatusCode == StatusCode.Cancelled)
                {
                    WriteResponse("Unsubscription successful");
                }
                catch (OperationCanceledException)
                {
                    WriteResponse("Unsubscription successful");
                }
        }

            private void WriteResponse(string response)
            {
                lock (this.stringbuider)
                {
                    this.stringbuider.AppendLine($"{DateTime.Now.Ticks}: {response}");
                    ServerResponse = this.stringbuider.ToString();
                }
            }

            public async Task Unsubscribe()
            {
                await this.grpcClient.UnsubscribeForGloeggliAsync(this.ellieRequest);
            }
    }
}