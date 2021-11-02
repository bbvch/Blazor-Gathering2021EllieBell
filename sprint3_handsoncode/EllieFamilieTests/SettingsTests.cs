using Blazored.LocalStorage;
using Bunit;
using EllieFamilie.Notification;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EllieFamilieTests
{
    public class SettingsTests
    {
        private TestContext testContext = new();

        public SettingsTests()
        {
            testContext.Services.AddSingleton<ILocalStorageService, MockLocalStorageService>();
        }

        [Fact]
        public async Task SaveStoresValueToLocalStorage()
        {
            var testee = testContext.RenderComponent<Settings>();
            testee.Find("#clientId").Change("123");
            testee.Find("#clientName").Change("Viktor");

            testee.Find("#save").Click();

            var _ = new AssertionScope();
            var localStorage = testContext.Services.GetService<ILocalStorageService>();
            var actualName = await localStorage.GetItemAsync<string>("Name");
            actualName.Should().Be("Viktor");
            var actualId = await localStorage.GetItemAsync<int>("Id");
            actualId.Should().Be(123);
        }

        [Fact]
        public async Task ClearEmptiesLocalStorage()
        {
            var localStorage = testContext.Services.GetService<ILocalStorageService>();
            await localStorage.SetItemAsync<string>("test", "juhu");

            var testee = testContext.RenderComponent<Settings>();
            testee.Find("#clear").Click();

            (await localStorage.ContainKeyAsync("test")).Should().BeFalse();
        }    

        [Fact]
        public async Task InitReadsFromLocalStorage()
        {
            var localStorage = testContext.Services.GetService<ILocalStorageService>();
            await localStorage.SetItemAsync<int>("Id", 4711);
            await localStorage.SetItemAsync<string>("Name", "juhu");

            var testee = testContext.RenderComponent<Settings>();

            testee.Find("#clientId").GetAttribute("value").Should().Be("4711");
            testee.Find("#clientName").GetAttribute("value").Should().Be("juhu");
        } 
    }
}