namespace EllieFamilie.Notification
{
    public class UserInfo
    {
        public int Id;
        public string Name = string.Empty;
    }

    public partial class Settings
    {
        private UserInfo UserInfo { get; set; } = new UserInfo();

        protected override async Task OnInitializedAsync()
        {
            if (await localStore.ContainKeyAsync(nameof(UserInfo.Name)))
            {
                UserInfo.Name = await localStore.GetItemAsync<string>(nameof(UserInfo.Name));
            }
            if (await localStore.ContainKeyAsync(nameof(UserInfo.Id)))
            {
                UserInfo.Id = await localStore.GetItemAsync<int>(nameof(UserInfo.Id));
            }        
        }

        public async void UpdateLocalStorage()
        {
            await localStore.SetItemAsync(nameof(UserInfo.Id), UserInfo.Id);
            await localStore.SetItemAsync(nameof(UserInfo.Name), UserInfo.Name);
        }

        public async void ClearLocalStorage()
        {
            UserInfo = new UserInfo();
            await localStore.ClearAsync();
        }
    }
}
