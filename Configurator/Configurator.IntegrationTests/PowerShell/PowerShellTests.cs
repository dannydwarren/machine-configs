using AutoMoqCore;
using Configurator.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Configurator.IntegrationTests.PowerShell
{
    public class PowerShellTests : IntegrationTestBase<Configurator.PowerShell.PowerShell>
    {
        private readonly Mock<IConsoleLogger> mockConsoleLogger;

        public PowerShellTests()
        {
            AutoMoqer mocker = new AutoMoqer();
            mockConsoleLogger = mocker.GetMock<IConsoleLogger>();

            Services.AddTransient(_ => mockConsoleLogger.Object);
        }

        [Fact]
        public async Task When_writing_to_debug()
        {
            var script = @"$DebugPreference = 'Continue'
Write-Debug 'Hello World'
Write-Debug 'Hello World'";

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync(script));

            It("logs as debug", () =>
            {
                mockConsoleLogger.Verify(x => x.Debug("Hello World"), Times.Exactly(2));
            });
        }

        [Fact]
        public async Task When_writing_to_verbose()
        {
            var script = @"$VerbosePreference = 'Continue'
Write-Verbose 'Hello World'
Write-Verbose 'Hello World'";

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync(script));

            It("logs as info", () =>
            {
                mockConsoleLogger.Verify(x => x.Verbose("Hello World"), Times.Exactly(2));
            });
        }

        [Fact]
        public async Task When_writing_to_host()
        {
            var script = @"Write-Host 'Hello World'
Write-Host 'Hello World'";

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync(script));

            It("logs as info", () =>
            {
                mockConsoleLogger.Verify(x => x.Info("Hello World"), Times.Exactly(2));
            });
        }

        [Fact]
        public async Task When_writing_to_information()
        {
            var script = @"Write-Information 'Hello World'
Write-Information 'Hello World'";

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync(script));

            It("logs as info", () =>
            {
                mockConsoleLogger.Verify(x => x.Info("Hello World"), Times.Exactly(2));
            });
        }

        [Fact]
        public async Task When_writing_to_warning()
        {
            var script = @"Write-Warning 'Hello World'
Write-Warning 'Hello World'";

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync(script));

            It("logs as warn", () =>
            {
                mockConsoleLogger.Verify(x => x.Warn("Hello World"), Times.Exactly(2));
            });
        }

        [Fact]
        public async Task When_writing_to_error()
        {
            var script = @"Write-Error 'Hello World'
Write-Error 'Hello World'";

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync(script));

            It("logs as error", () =>
            {
                mockConsoleLogger.Verify(x => x.Error("Hello World"), Times.Exactly(2));
            });
        }

        [Fact]
        public async Task When_writing_progress()
        {
            var script = @"Write-Progress -Activity 'Hello World' -Status '1 Complete' -PercentComplete 50
Write-Progress -Activity 'Hello World' -Status '2 Complete' -PercentComplete 100";

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync(script));

            It("logs as info", () =>
            {
                mockConsoleLogger.Verify(x => x.Progress($"Hello World -> Status: 1 Complete; PercentComplete: 50"));
                mockConsoleLogger.Verify(x => x.Progress($"Hello World -> Status: 2 Complete; PercentComplete: 100"));
            });
        }

        [Fact]
        public async Task When_executing()
        {
            var script = @"Write-Output ""Hello World""";

            var output = await BecauseAsync(() => ClassUnderTest.ExecuteAsync(script));

            It("returns output", () =>
            {
                output.AsString.ShouldBe("Hello World");
            });
        }

        [Fact]
        public async Task When_executing_script_for_the_first_time_with_complete_check()
        {
            var script = @"$testVar = $false
Write-Information ""testVar=$testVar""
$testVar = $true
Write-Information ""testVar=$testVar""";
            var completeCheckScript = "$testVar -eq $true";

            var output = await BecauseAsync(() => ClassUnderTest.ExecuteAsync(script, completeCheckScript));

            It("returns output", () =>
            {
                mockConsoleLogger.Verify(x => x.Info("testVar=False"));
                mockConsoleLogger.Verify(x => x.Info("testVar=True"));
                output.AsBool.ShouldNotBeNull().ShouldBeTrue();
            });
        }

        [Fact]
        public async Task When_executing_script_that_has_already_been_completed()
        {
            var script = @"Write-Information 'Should not get this'";
            var completeCheckScript = "$true";

            var output = await BecauseAsync(() => ClassUnderTest.ExecuteAsync(script, completeCheckScript));

            It("returns output", () =>
            {
                mockConsoleLogger.Verify(x => x.Info("Should not get this"), Times.Exactly(0));
                output.AsBool.ShouldNotBeNull().ShouldBeTrue();
            });
        }

        [Fact]
        public async Task When_executing_script_for_the_first_time_with_failing_complete_check()
        {
            var script = @"Write-Information 'Script run'";
            var completeCheckScript = "$false";

            var output = await BecauseAsync(() => ClassUnderTest.ExecuteAsync(script, completeCheckScript));

            It("returns output", () =>
            {
                mockConsoleLogger.Verify(x => x.Info("Script run"));
                output.AsBool.ShouldNotBeNull().ShouldBeFalse();
            });
        }

        [Fact]
        public async Task When_executing_script_for_the_first_time_with_complete_check_missing_result()
        {
            var script = @"Write-Information 'Script run'";
            var completeCheckScript = "'NotTrue'";

            var output = await BecauseAsync(() => ClassUnderTest.ExecuteAsync(script, completeCheckScript));

            It("returns output", () =>
            {
                mockConsoleLogger.Verify(x => x.Info("Script run"));
                output.AsString.ShouldBe("NotTrue");
                output.AsBool.ShouldBeNull();
            });
        }

        [Fact]
        public async Task When_executing_a_script_PS_profile_variable_is_populated()
        {
            var script = @"Write-Information ""Profile path: $profile""";

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync(script));

            It("has $profile populated", () =>
            {
                mockConsoleLogger.Verify(x => x.Info(Moq.It.Is<string>(y => y.EndsWith("profile.ps1"))), Times.Exactly(1));
            });
        }
    }
}
