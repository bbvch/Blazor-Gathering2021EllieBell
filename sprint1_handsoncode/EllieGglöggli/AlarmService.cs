namespace EllieGglöggli
{
    public class AlarmService
    {
        public string AlarmSent { get; set; } = string.Empty;

        public async Task SendAlarmAsync()
        {
            AlarmSent += $"{DateTime.Now.ToShortTimeString()} Alarm sent to: 'local'{Environment.NewLine}";
            await Task.CompletedTask;
        }
    }
}
