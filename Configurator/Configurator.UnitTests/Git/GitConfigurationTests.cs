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
            var gitconfigPath = $"{RandomString()}\\{RandomString()}";
            var gitconfigPathEscaped = gitconfigPath.Replace(@"\", @"\\");
            var powerShellResult = new PowerShellResult { AsString = true.ToString() };
            
            GetMock<IPowerShell>().Setup(x => x.ExecuteAsync(Is<string>(x => x.Contains(gitconfigPath)), Is<string>(x => x.Contains(gitconfigPathEscaped)))).ReturnsAsync(powerShellResult);

            var result = await BecauseAsync(() => ClassUnderTest.IncludeGitconfigAsync(gitconfigPath));

            It("includes the custom path", () =>
            {
                GetMock<IConsoleLogger>().Verify(x => x.Info($"Including gitconfig: {gitconfigPath}"));
                result.ShouldBeTrue();
                GetMock<IConsoleLogger>().Verify(x => x.Result($"Included gitconfig: {gitconfigPath}"));
            });
        }
    }
}
