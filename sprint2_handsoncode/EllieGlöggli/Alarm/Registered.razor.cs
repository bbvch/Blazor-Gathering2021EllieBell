using EllieGlöggli.Common;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;


namespace EllieGlöggli.Alarm
{
    public partial class Registered
    {

        [Inject]
        private AlarmService AlarmService { get; set; }

        public IReadOnlyCollection<RegisterRequest> Users { get; set; } = new ReadOnlyCollection<RegisterRequest>(new List<RegisterRequest>());

        protected override async Task OnInitializedAsync()
        {
            Users = await AlarmService.GetRegisteredAsync();
            await base.OnInitializedAsync();
        }
    }
}
