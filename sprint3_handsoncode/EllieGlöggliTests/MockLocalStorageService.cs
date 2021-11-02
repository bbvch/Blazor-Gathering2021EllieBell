using Blazored.LocalStorage;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EllieGlöggliTests
{
    public class MockLocalStorageService : ILocalStorageService
    {
        private Dictionary<string, object> storage = new();

        public event System.EventHandler<ChangingEventArgs> Changing;
        public event System.EventHandler<ChangedEventArgs> Changed;

        public async ValueTask ClearAsync(CancellationToken? cancellationToken = null)
        {
            storage = new();
            await ValueTask.CompletedTask;
        }

        public async ValueTask<bool> ContainKeyAsync(string key, CancellationToken? cancellationToken = null)
        {
            return await ValueTask.FromResult(storage.ContainsKey(key));
        }

        public ValueTask<string> GetItemAsStringAsync(string key, CancellationToken? cancellationToken = null)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<T> GetItemAsync<T>(string key, CancellationToken? cancellationToken = null)
        {
            return ValueTask.FromResult((T)(dynamic)storage[key]);
        }

        public ValueTask<string> KeyAsync(int index, CancellationToken? cancellationToken = null)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<int> LengthAsync(CancellationToken? cancellationToken = null)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask RemoveItemAsync(string key, CancellationToken? cancellationToken = null)
        {
            throw new System.NotImplementedException();
        }

        public async ValueTask SetItemAsStringAsync(string key, string data, CancellationToken? cancellationToken = null)
        {
            storage.Add(key, data?.ToString() ?? string.Empty);
            await ValueTask.CompletedTask;
        }

        public async ValueTask SetItemAsync<T>(string key, T data, CancellationToken? cancellationToken = null)
        {
            storage.Add(key, data);
            await ValueTask.CompletedTask;
        }
    }
}