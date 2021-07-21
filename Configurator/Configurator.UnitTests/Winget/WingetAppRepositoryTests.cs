using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Configurator.Utilities;
using Configurator.Winget;
using Moq;
using Shouldly;
using Xunit;

namespace Configurator.UnitTests.Winget
{
    public class WingetAppRepositoryTests : UnitTestBase<WingetAppRepository>
    {
        [Fact]
        public async Task When_loading_apps_to_install_with_winget()
        {
            var wingetAppsPath = RandomString();
            var appId1 = RandomString();
            var appId2 = RandomString();
            var appId3 = RandomString();
            var appId4 = RandomString();

            var csv = new List<string>
            {
                $"{appId1},{InstallEnvironment.Personal}",
                $"{appId2},{InstallEnvironment.Work}",
                $"{appId3},{InstallEnvironment.Personal|InstallEnvironment.Work}",
                $"{appId4},{InstallEnvironment.All}",
            };


            GetMock<IArguments>().SetupGet(x => x.WingetAppsPath).Returns(wingetAppsPath);
            GetMock<IArguments>().SetupGet(x => x.Environment).Returns(InstallEnvironment.Personal);
            GetMock<IFileSystem>().Setup(x => x.ReadAllLinesAsync(wingetAppsPath)).ReturnsAsync(csv);

            var apps = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("loads winget apps for the environment", () =>
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
            var wingetAppsPath = RandomString();

            var csv = new List<string>
            {
                $"{RandomString()},",
            };

            GetMock<IArguments>().SetupGet(x => x.WingetAppsPath).Returns(wingetAppsPath);
            GetMock<IFileSystem>().Setup(x => x.ReadAllLinesAsync(wingetAppsPath)).ReturnsAsync(csv);

            var exception = await BecauseThrowsAsync<Exception>(() => ClassUnderTest.LoadAsync());

            It("provides a useful error message", () =>
            {
                exception.ShouldNotBeNull()
                    .Message.ShouldBe($"Malformed winget app on line 1 [missing parts]: {csv[0]}");
            });
        }

        [Fact]
        public async Task When_loading_an_app_with_invalid_installation_environment()
        {
            var wingetAppsPath = RandomString();

            var csv = new List<string>
            {
                $"{RandomString()},Fake",
            };

            GetMock<IArguments>().SetupGet(x => x.WingetAppsPath).Returns(wingetAppsPath);
            GetMock<IFileSystem>().Setup(x => x.ReadAllLinesAsync(wingetAppsPath)).ReturnsAsync(csv);

            var exception = await BecauseThrowsAsync<Exception>(() => ClassUnderTest.LoadAsync());

            It("provides a useful error message", () =>
            {
                exception.ShouldNotBeNull()
                    .Message.ShouldBe($"Malformed winget app on line 1 [invalid install environment]: {csv[0]}");
            });
        }
    }
}
