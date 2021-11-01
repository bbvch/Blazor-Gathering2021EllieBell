using EllieGlöggli.Common;
using Microsoft.AspNetCore.Components;

namespace EllieGlöggli.Alarm
{
    public partial class AlarmGlöggli : IAsyncDisposable
    {
        private const int EllieClientId = 1001;
        private readonly System.Text.StringBuilder stringBuilder = new();

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
            GC.SuppressFinalize(this);
            await Unsubscribe();
        }

        public void Subscribe()
        {
            AlarmService.StateChanged += AlarmService_StateChanged;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            AlarmService.Subscribe(new RegisterRequest { ClientId = EllieClientId, Name = "I'm Ellie" });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private void AlarmService_StateChanged(object sender, ResponseEventArgs e)
        {
            WriteResponse(e.Response);
        }

        public async Task Unsubscribe()
        {
            await AlarmService.Unsubscribe(new RegisterRequest { ClientId = EllieClientId });
            AlarmService.StateChanged -= AlarmService_StateChanged;
        }

        public async Task FireAlarmAsync()
        {
            await AlarmService.FireAlarmAsync(new GloeggeliRequest { SenderClientId = EllieClientId });
        }

        private void WriteResponse(string response)
        {
            lock (stringBuilder)
            {
                stringBuilder.AppendLine($"{DateTime.Now.Ticks}: {response}");
                ServerResponse = stringBuilder.ToString();
                StateHasChanged();
            }
        }
    }
}
