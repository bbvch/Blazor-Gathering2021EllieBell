using EllieGlöggli.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EllieGlöggliTests
{
    internal class MockAlarmService : IAlarmService
    {
        public event AlarmService.StateChangedHandler? StateChanged;

        public async Task FireAlarmAsync(GloeggeliRequest gloeggeliRequest)
        {
            StateChanged?.Invoke(this, new ResponseEventArgs { Response = $"{gloeggeliRequest.SenderClientId}" });
            await Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<RegisterRequest>> GetRegisteredAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task Subscribe(RegisterRequest registerRequest)
        {
            StateChanged?.Invoke(this, new ResponseEventArgs { Response = $"{registerRequest.Name}" });
            await Task.CompletedTask; 
        }

        public async Task Unsubscribe(RegisterRequest registerRequest)
        {
            StateChanged?.Invoke(this, new ResponseEventArgs { Response = $"{registerRequest.Name}" });
            await Task.CompletedTask;
        }
    }
}