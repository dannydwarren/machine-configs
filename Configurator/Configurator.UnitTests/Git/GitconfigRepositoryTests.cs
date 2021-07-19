using Configurator.Git;
using Configurator.Utilities;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Configurator.UnitTests.Git
{
    public class GitconfigRepositoryTests : UnitTestBase<GitconfigRepository>
    {
        [Fact]
        public async Task When_loading()
        {
            var gitconfigsPath = RandomString();

            var path1 = RandomString();
            var path2 = RandomString();
            var csv = new List<string>
            {
                $"{path1},{InstallEnvironment.Personal}",
                $"{path2},{InstallEnvironment.Work}",
            };

            GetMock<IArguments>().SetupGet(x => x.Environment).Returns(InstallEnvironment.Personal);
            GetMock<IArguments>().SetupGet(x => x.GitconfigsPath).Returns(gitconfigsPath);
            GetMock<IFileSystem>().Setup(x => x.ReadAllLinesAsync(gitconfigsPath)).ReturnsAsync(csv);

            var gitconfigs = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("loads gitconfigs for the environment", () =>
            {
                var item = gitconfigs.ShouldHaveSingleItem();
                item.Path.ShouldBe(path1);
                item.Environment.ShouldBe(InstallEnvironment.Personal);
            });
        }

        [Fact]
        public async Task When_loading_a_malformed_gitconfig()
        {
            var gitconfigsPath = RandomString();

            var csv = new List<string>
            {
                $"{RandomString()},",
            };


            GetMock<IArguments>().SetupGet(x => x.GitconfigsPath).Returns(gitconfigsPath);
            GetMock<IFileSystem>().Setup(x => x.ReadAllLinesAsync(gitconfigsPath)).ReturnsAsync(csv);

            var exception = await BecauseThrowsAsync<Exception>(() => ClassUnderTest.LoadAsync());

            It("provides a useful error message", () =>
            {
                exception.ShouldNotBeNull()
                    .Message.ShouldBe($"Malformed gitconfig on line 1 [missing parts]: {csv[0]}");
            });
        }

        [Fact]
        public async Task When_loading_an_gitconfig_with_invalid_installation_environment()
        {
            var gitconfigsPath = RandomString();

            var csv = new List<string>
            {
                $"{RandomString()},Fake",
            };


            GetMock<IArguments>().SetupGet(x => x.GitconfigsPath).Returns(gitconfigsPath);
            GetMock<IFileSystem>().Setup(x => x.ReadAllLinesAsync(gitconfigsPath)).ReturnsAsync(csv);

            var exception = await BecauseThrowsAsync<Exception>(() => ClassUnderTest.LoadAsync());

            It("provides a useful error message", () =>
            {
                exception.ShouldNotBeNull()
                    .Message.ShouldBe($"Malformed gitconfig on line 1 [invalid install environment]: {csv[0]}");
            });
        }
    }
}
