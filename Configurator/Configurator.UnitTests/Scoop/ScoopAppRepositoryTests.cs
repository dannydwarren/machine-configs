using Configurator.Scoop;
using Configurator.Utilities;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Configurator.UnitTests.Scoop
{
    public class ScoopAppRepositoryTests : UnitTestBase<ScoopAppRepository>
    {
        [Fact]
        public async Task When_loading_apps_to_install_with_scoop()
        {
            var scoopAppsPath = RandomString();
            var appId1 = RandomString();
            var appId2 = RandomString();
            var appId3 = RandomString();
            var appId4 = RandomString();

            var csvLines = new List<string>
            {
                $"{appId1},{InstallEnvironment.Personal}",
                $"{appId2},{InstallEnvironment.Work}",
                $"{appId3},{InstallEnvironment.Personal|InstallEnvironment.Work}",
                $"{appId4},{InstallEnvironment.All}",
            };


            GetMock<IArguments>().SetupGet(x => x.ScoopAppsPath).Returns(scoopAppsPath);
            GetMock<IArguments>().SetupGet(x => x.Environment).Returns(InstallEnvironment.Personal);
            GetMock<IFileSystem>().Setup(x => x.ReadAllLinesAsync(scoopAppsPath)).ReturnsAsync(csvLines);

            var apps = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses scoop apps", () =>
            {
                apps.Count.ShouldBe(3);
                apps[0].AppId.ShouldBe(appId1);
                apps[0].Environment.ShouldBe(InstallEnvironment.Personal);
                apps[1].AppId.ShouldBe(appId3);
                apps[1].Environment.ShouldBe(InstallEnvironment.Personal | InstallEnvironment.Work);
                apps[2].AppId.ShouldBe(appId4);
                apps[2].Environment.ShouldBe(InstallEnvironment.All);
            });
        }

        [Fact]
        public async Task When_loading_a_malformed_app()
        {
            var scoopAppsPath = RandomString();

            var csvLines = new List<string>
            {
                $"{RandomString()},",
            };


            GetMock<IArguments>().SetupGet(x => x.ScoopAppsPath).Returns(scoopAppsPath);
            GetMock<IFileSystem>().Setup(x => x.ReadAllLinesAsync(scoopAppsPath)).ReturnsAsync(csvLines);

            var exception = await BecauseThrowsAsync<Exception>(() => ClassUnderTest.LoadAsync());

            It("provices a useful error message", () =>
            {
                exception.ShouldNotBeNull()
                    .Message.ShouldBe($"Malformed scoop app on line 1 [missing parts]: {csvLines[0]}");
            });
        }

        [Fact]
        public async Task When_loading_an_app_with_invalid_installation_environment()
        {
            var scoopAppsPath = RandomString();

            var csvLines = new List<string>
            {
                $"{RandomString()},Fake",
            };


            GetMock<IArguments>().SetupGet(x => x.ScoopAppsPath).Returns(scoopAppsPath);
            GetMock<IFileSystem>().Setup(x => x.ReadAllLinesAsync(scoopAppsPath)).ReturnsAsync(csvLines);

            var exception = await BecauseThrowsAsync<Exception>(() => ClassUnderTest.LoadAsync());

            It("provices a useful error message", () =>
            {
                exception.ShouldNotBeNull()
                    .Message.ShouldBe($"Malformed scoop app on line 1 [invalid install environment]: {csvLines[0]}");
            });
        }
    }
}
