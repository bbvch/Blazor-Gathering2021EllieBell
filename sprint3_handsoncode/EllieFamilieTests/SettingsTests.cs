using Blazored.LocalStorage;
using Bunit;
using EllieFamilie.Notification;
using EllieGlöggli.Common.Admin;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EllieFamilieTests
{
    public class SettingsTests
    {
        private TestContext testContext = new();
        private ILocalStorageService localStorage;

        public SettingsTests()
        {
            testContext.Services.AddLocalization();
            testContext.Services.AddEllieAdmin();

            testContext.Services.AddSingleton<ILocalStorageService, MockLocalStorageService>();
            localStorage = testContext.Services.GetService<ILocalStorageService>()!;
        }

        [Fact]
        public async Task SaveStoresValueToLocalStorage()
        {
            var testee = testContext.RenderComponent<EllieFamilie.Notification.Settings>();
            testee.Find("#clientId").Change("123");
            testee.Find("#clientName").Change("Viktor");

            testee.Find("#save").Click();

            var actualId = await localStorage.GetItemAsync<int>("Id");
            var actualName = await localStorage.GetItemAsync<string>("Name");
            actualId.Should().Be(123);
            actualName.Should().Be("Viktor");
        }

        [Fact]
        public async Task ClearEmptiesLocalStorage()
        {
            await localStorage.SetItemAsync<string>("test", "juhu");

            var testee = testContext.RenderComponent<EllieFamilie.Notification.Settings>();
            testee.Find("#clear").Click();

            (await localStorage.ContainKeyAsync("test")).Should().BeFalse();
        }    

        [Fact]
        public async Task InitReadsFromLocalStorage()
        {
            await localStorage.SetItemAsync<int>("Id", 4711);
            await localStorage.SetItemAsync<string>("Name", "juhu");

            var testee = testContext.RenderComponent<EllieFamilie.Notification.Settings>();

            testee.Find("#clientId").GetAttribute("value").Should().Be("4711");
            testee.Find("#clientName").GetAttribute("value").Should().Be("juhu");
        }

        [Fact]
        public void LocalizationAllEnglishTranslationsExists()
        {
            var testee = testContext.RenderComponent<EllieFamilie.Notification.Settings>();

            testee.Markup.Should().NotContain("***");
        }

        [Fact]
        public void LocalizationAllGermanTranslationsExists()
        {
            CultureInfo.CurrentUICulture = CultureInfo.CreateSpecificCulture("de");

            var testee = testContext.RenderComponent<EllieFamilie.Notification.Settings>();

            testee.Markup.Should().NotContain("***");
        }

        [Fact]
        public void LocalizationTitleShouldBeInEnglish()
        {
            var testee = testContext.RenderComponent<EllieFamilie.Notification.Settings>();

            testee.Find("h3").MarkupMatches("<h3>Settings</h3>");
        }

        [Fact]
        public void LocalizationTitleShouldBeInGerman()
        {
            CultureInfo.CurrentUICulture = CultureInfo.CreateSpecificCulture("de"); 

            var testee = testContext.RenderComponent<EllieFamilie.Notification.Settings>();

            testee.Find("h3").MarkupMatches("<h3>Einstellungen</h3>");
        }
    }
}