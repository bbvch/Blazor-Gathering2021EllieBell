using Blazored.LocalStorage;

namespace EllieGlöggli.Common.Admin
{
    public class UserInfoService
    {
        private readonly ILocalStorageService localStore;

        public UserInfoService(ILocalStorageService localStore)
        {
            this.localStore = localStore;
        }

        public async Task<UserInfo> LoadAsync()
        {
            var userInfo = new UserInfo();

            if (await localStore.ContainKeyAsync(nameof(userInfo.Name)))
            {
                userInfo.Name = await localStore.GetItemAsync<string>(nameof(userInfo.Name));
            }
            if (await localStore.ContainKeyAsync(nameof(userInfo.Id)))
            {
                userInfo.Id = await localStore.GetItemAsync<int>(nameof(userInfo.Id));
            }

            return userInfo;
        }

        internal async Task SaveAsync(UserInfo userInfo)
        {
            await localStore.SetItemAsync(nameof(UserInfo.Id), userInfo.Id);
            await localStore.SetItemAsync(nameof(UserInfo.Name), userInfo.Name);
        }

        internal async Task ClearAsync()
        {
            await localStore.ClearAsync();
        }
    }
}
