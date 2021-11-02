using Blazored.LocalStorage;
using Bunit;
using EllieGlöggli.Alarm;
using EllieGlöggli.Common;
using EllieGlöggli.Common.Admin;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EllieGlöggliTests
{
    public class AlarmGlöggliTests
    {
        private TestContext testContext = new();
        private readonly IRenderedComponent<AlarmGlöggli> testee;

        public AlarmGlöggliTests()
        {
            testContext.Services.AddSingleton<IAlarmService, MockAlarmService>();
            testContext.Services.AddLocalization();
            testContext.Services.AddSingleton<ILocalStorageService, MockLocalStorageService>();
            testContext.Services.AddTransient<UserInfoService>();

            var storage = testContext.Services.GetService<ILocalStorageService>()!;
            storage.SetItemAsync<int>(nameof(UserInfo.Id), 1001);
            storage.SetItemAsStringAsync(nameof(UserInfo.Name), "I'm Ellie");
            testee = testContext.RenderComponent<AlarmGlöggli>();
        }

        [Fact]
        public void InitSubcribes()
        {
            var actual = testee.Find("textarea").GetAttribute("value");
            actual.Should().Contain("I'm Ellie");
        }

        [Fact]
        public void ClickAlarmReturnsSentText()
        {
            testee.Find("button").Click();

            var actual = testee.Find("textarea").GetAttribute("value");
            actual.Should().Contain("I'm Ellie");
            actual.Should().Contain("1001");
        }
    }
}