using Configurator.Git;
using Configurator.PowerShell;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Shouldly;
using Configurator.Utilities;

namespace Configurator.UnitTests.Git
{
    public class GitConfigurationTests : UnitTestBase<GitConfiguration>
    {
        [Fact]
        public async Task When_including_a_custom_gitconfig()
        {
            var gitconfigPath = RandomString();
            var powerShellResult = new PowerShellResult { AsString = true.ToString() };
            
            GetMock<IPowerShell>().Setup(x => x.ExecuteAsync(Is<string>(x => x.Contains(gitconfigPath)))).ReturnsAsync(powerShellResult);

            var result = await BecauseAsync(() => ClassUnderTest.IncludeCustomGitconfigAsync(gitconfigPath));

            It("includes the custom path", () =>
            {
                GetMock<IConsoleLogger>().Verify(x => x.Info("Including custom gitconfig"));
                result.ShouldBeTrue();
                GetMock<IConsoleLogger>().Verify(x => x.Result("Included custom gitconfig"));
            });
        }
    }
}
