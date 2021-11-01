using EllieGlöggli.Common;
using Microsoft.AspNetCore.Components;

namespace EllieFamilie.Notification
{
    public partial class AlarmNotification : IAsyncDisposable
    {
        private const int FamilieMemberClientId = 1002;
        private readonly System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        private bool isPlaySound = false;

        [Inject]
        private AlarmService AlarmService { get; set; }

        private string ServerResponse { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            Subscribe();
            await base.OnInitializedAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await Unsubscribe();
        }

        public void Subscribe()
        {
            AlarmService.StateChanged += AlarmService_StateChanged;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            AlarmService.Subscribe(new RegisterRequest { ClientId = FamilieMemberClientId, Name = "Viktor" });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private void AlarmService_StateChanged(object sender, ResponseEventArgs e)
        {
            WriteResponse(e.Response);
        }

        public async Task Unsubscribe()
        {
            await AlarmService.Unsubscribe(new RegisterRequest { ClientId = FamilieMemberClientId });
            AlarmService.StateChanged -= AlarmService_StateChanged;
        }

        private void WriteResponse(string response)
        {
            lock (stringBuilder)
            {
                stringBuilder.AppendLine($"{DateTime.Now.Ticks}: {response}");
                ServerResponse = stringBuilder.ToString();
                isPlaySound = response.Contains("Notification Recieved");
                StateHasChanged();
            }
        }
    }
}
