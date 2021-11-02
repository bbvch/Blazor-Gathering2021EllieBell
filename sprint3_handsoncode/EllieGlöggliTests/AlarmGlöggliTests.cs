using Bunit;
using EllieGlöggli.Alarm;
using EllieGlöggli.Common;
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