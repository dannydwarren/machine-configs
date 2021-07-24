using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMoqCore;
using Configurator.Apps;
using Configurator.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Xunit;

namespace Configurator.IntegrationTests.Apps
{
    public class AppsRepositoryTests : IntegrationTestBase<AppsRepository>
    {
        private readonly Mock<IArguments> mockArgs;

        public AppsRepositoryTests()
        {
            AutoMoqer mocker = new();
            mockArgs = mocker.GetMock<IArguments>();
            mockArgs.SetupGet(x => x.Environments).Returns(new List<string> {"Test"});
        }

        [Fact]
        public async Task When_parsing_scoop_apps()
        {
            mockArgs.SetupGet(x => x.AppsPath).Returns("./Apps/apps_scoop_only.json");

            Services.AddTransient(_ => mockArgs.Object);

            var apps = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses", () =>
            {
                apps.ScoopApps.ShouldHaveSingleItem().AppId.ShouldBe("scoop-app-id");
            });
        }

        [Fact]
        public async Task When_parsing_winget_apps()
        {
            mockArgs.SetupGet(x => x.AppsPath).Returns("./Apps/apps_winget_only.json");

            Services.AddTransient(_ => mockArgs.Object);

            var apps = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses", () =>
            {
                apps.WingetApps.ShouldHaveSingleItem().AppId.ShouldBe("winget-app-id");
            });
        }

        [Fact]
        public async Task When_parsing_non_package_apps()
        {
            mockArgs.SetupGet(x => x.AppsPath).Returns("./Apps/apps_non-package_only.json");

            Services.AddTransient(_ => mockArgs.Object);

            var apps = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses", () =>
            {
                apps.NonPackageApps.ShouldHaveSingleItem().AppId.ShouldBe("non-package-app-id");
            });
        }

        [Fact]
        public async Task When_parsing_power_shell_app_packages()
        {
            mockArgs.SetupGet(x => x.AppsPath).Returns("./Apps/apps_power-shell-app-package_only.json");

            Services.AddTransient(_ => mockArgs.Object);

            var apps = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses", () =>
            {
                apps.PowerShellAppPackages.ShouldHaveSingleItem().ShouldSatisfyAllConditions(x =>
                {
                    x.AppId.ShouldBe("power-shell-app-package-app-id");
                    x.Downloader.ShouldBe("some-downloader");
                    x.DownloaderArgs.ToString().ShouldNotBeEmpty();
                });
            });
        }
    }
}
