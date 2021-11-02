using EllieGlöggli.Common;
using Microsoft.AspNetCore.Components;

namespace EllieFamilie.Notification
{
    public partial class AlarmNotification : IAsyncDisposable
    {
        private readonly System.Text.StringBuilder stringBuilder = new();
        private bool isPlaySound = false;

        [Inject]
        private AlarmService AlarmService { get; set; }

        private string ServerResponse { get; set; } = string.Empty;
        private UserInfo UserInfo { get; set; } = new UserInfo();

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
            if (await localStore.ContainKeyAsync(nameof(UserInfo.Name)))
            {
                UserInfo.Name = await localStore.GetItemAsync<string>(nameof(UserInfo.Name));
            }
            if (await localStore.ContainKeyAsync(nameof(UserInfo.Id)))
            {
                UserInfo.Id = await localStore.GetItemAsync<int>(nameof(UserInfo.Id));
            }
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
