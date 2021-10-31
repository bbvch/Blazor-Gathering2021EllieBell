namespace EllieFamilie
{
    public class NotificationService
    {
        public delegate void StateChangedHandler(object sender, EventArgs e);
        public event StateChangedHandler? StateChanged;

        public string AlarmReceived { get; set; } = string.Empty;

        public async Task ReceiveAlarmAsync()
        {
            AlarmReceived += $"{DateTime.Now.ToShortTimeString()} Alarm received from: 'local'{Environment.NewLine}";
            StateChanged?.Invoke(this, EventArgs.Empty);
            await Task.CompletedTask;
        }
    }
}
