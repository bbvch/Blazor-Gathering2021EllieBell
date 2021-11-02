using EllieGlöggli.Common;
using EllieGlöggli.Common.Admin;
using Microsoft.AspNetCore.Components;

namespace EllieGlöggli.Alarm
{
    public partial class AlarmGlöggli : IAsyncDisposable
    {
        private readonly System.Text.StringBuilder stringBuilder = new();
        private UserInfo UserInfo { get; set; } = new UserInfo();

        [Inject]
        private IAlarmService AlarmService { get; set; }

        [Inject]
        public UserInfoService UserInfoService { get; set; }

        private string ServerResponse { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await Subscribe();
            await base.OnInitializedAsync();
        }

        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            await Unsubscribe();
        }

        public async Task Subscribe()
        {
            UserInfo = await UserInfoService.LoadAsync();

            AlarmService.StateChanged += AlarmService_StateChanged;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            AlarmService.Subscribe(new RegisterRequest { ClientId = UserInfo.Id, Name = UserInfo.Name });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private void AlarmService_StateChanged(object sender, ResponseEventArgs e)
        {
            WriteResponse(e.Response);
        }

        public async Task Unsubscribe()
        {
            await AlarmService.Unsubscribe(new RegisterRequest { ClientId = UserInfo.Id });
            AlarmService.StateChanged -= AlarmService_StateChanged;
        }

        public async Task FireAlarmAsync()
        {
            await AlarmService.FireAlarmAsync(new GloeggeliRequest { SenderClientId = UserInfo.Id });
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
