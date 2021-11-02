using Microsoft.AspNetCore.Components;

namespace EllieGlöggli.Common.Admin
{
    public partial class Settings
    {
        private UserInfo UserInfo { get; set; } = new UserInfo();
        
        [Inject]
        public UserInfoService UserInfoService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            UserInfo = await UserInfoService.LoadAsync();
        }

        public async Task UpdateLocalStorage()
        {
            await UserInfoService.SaveAsync(UserInfo);
        }

        public async Task ClearLocalStorage()
        {
            UserInfo = new UserInfo();
            await UserInfoService.ClearAsync();
        }
    }
}
