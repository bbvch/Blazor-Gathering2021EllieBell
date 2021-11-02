
namespace EllieGlöggli.Common
{
    public interface IAlarmService
    {
        event AlarmService.StateChangedHandler? StateChanged;

        Task FireAlarmAsync(GloeggeliRequest gloeggeliRequest);
        Task<IReadOnlyCollection<RegisterRequest>> GetRegisteredAsync();
        Task Subscribe(RegisterRequest registerRequest);
        Task Unsubscribe(RegisterRequest registerRequest);
    }
}