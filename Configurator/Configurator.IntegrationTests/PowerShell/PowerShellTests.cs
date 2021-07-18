using Configurator.Configuration;
using Moq;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Configurator.IntegrationTests.PowerShell
{
    public class PowerShellTests : IntegrationTestBase<Configurator.PowerShell.PowerShell>
    {
        [Fact]
        public async Task When_writing_to_debug()
        {
            var script = @"$DebugPreference = 'Continue'
Write-Debug 'Hello World'
Write-Debug 'Hello World'";

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync(script));

            It("logs as debug", () =>
            {
                GetMock<IConsoleLogger>().Verify(x => x.Debug("Hello World"), Times.Exactly(2));
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
                GetMock<IConsoleLogger>().Verify(x => x.Verbose("Hello World"), Times.Exactly(2));
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
                GetMock<IConsoleLogger>().Verify(x => x.Info("Hello World"), Times.Exactly(2));
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
                GetMock<IConsoleLogger>().Verify(x => x.Info("Hello World"), Times.Exactly(2));
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
                GetMock<IConsoleLogger>().Verify(x => x.Warn("Hello World"), Times.Exactly(2));
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
                GetMock<IConsoleLogger>().Verify(x => x.Error("Hello World"), Times.Exactly(2));
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
                GetMock<IConsoleLogger>().Verify(x => x.Progress($"Hello World -> Status: 1 Complete; PercentComplete: 50"));
                GetMock<IConsoleLogger>().Verify(x => x.Progress($"Hello World -> Status: 2 Complete; PercentComplete: 100"));
            });
        }

        [Fact]
        public async Task When_writing_to_output()
        {
            var script = @"Write-Output 'Hello World'";

            var output = await BecauseAsync(() => ClassUnderTest.ExecuteAsync(script));

            It("captured as output", () =>
            {
                output.AsString.ShouldBe("Hello World");
            });
        }
    }
}
