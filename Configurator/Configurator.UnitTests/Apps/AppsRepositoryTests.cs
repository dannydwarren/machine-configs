using System.Collections.Generic;
using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.Configuration;
using Configurator.Utilities;
using Moq;
using Shouldly;
using Xunit;

namespace Configurator.UnitTests.Apps
{
    public class AppsRepositoryTests : UnitTestBase<AppsRepository>
    {
        private Configurator.Apps.Apps loadedApps;

        public AppsRepositoryTests()
        {
            loadedApps = new Configurator.Apps.Apps
            {
                WingetApps = new()
                {
                    new WingetApp {AppId = RandomString(), Environments = "Personal".ToLower()},
                    new WingetApp {AppId = RandomString(), Environments = "Media"},
                    new WingetApp {AppId = RandomString(), Environments = "Work"},
                    new WingetApp {AppId = RandomString(), Environments = "All"}
                },
                ScoopApps = new()
                {
                    new ScoopApp {AppId = RandomString(), Environments = "Personal"},
                    new ScoopApp {AppId = RandomString(), Environments = "Media"},
                    new ScoopApp {AppId = RandomString(), Environments = "Work"},
                    new ScoopApp {AppId = RandomString(), Environments = "All"}
                },
                NonPackageApps = new()
                {
                    new NonPackageApp {AppId = RandomString(), Environments = "Personal"},
                    new NonPackageApp {AppId = RandomString(), Environments = "Media"},
                    new NonPackageApp {AppId = RandomString(), Environments = "Work"},
                    new NonPackageApp {AppId = RandomString(), Environments = "All"}
                },
                PowerShellAppPackages = new()
                {
                    new PowerShellAppPackage {AppId = RandomString(), Environments = "Personal"},
                    new PowerShellAppPackage {AppId = RandomString(), Environments = "Media"},
                    new PowerShellAppPackage {AppId = RandomString(), Environments = "Work"},
                    new PowerShellAppPackage {AppId = RandomString(), Environments = "All"}
                }
            };
        }

        [Fact]
        public async Task When_loading()
        {
            var appsPath = RandomString();
            var appsJson = RandomString();
            var specifiedEnvironment = new List<string> {"Personal", "Work"};
            var excludedEnvironment = "Media";

            GetMock<IArguments>().SetupGet(x => x.AppsPath).Returns(appsPath);
            GetMock<IArguments>().SetupGet(x => x.Environments).Returns(specifiedEnvironment);
            GetMock<IFileSystem>().Setup(x => x.ReadAllTextAsync(appsPath)).ReturnsAsync(appsJson);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<Configurator.Apps.Apps>(appsJson)).Returns(loadedApps);

            var apps = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It($"filters {nameof(apps.WingetApps)} by the specified environment", () =>
            {
                apps.WingetApps.ShouldAllBe(x => x.Environments != excludedEnvironment);
                apps.WingetApps.Count.ShouldBe(3);
            });

            It($"filters {nameof(apps.ScoopApps)} by the specified environment", () =>
            {
                apps.ScoopApps.ShouldAllBe(x => x.Environments != excludedEnvironment);
                apps.ScoopApps.Count.ShouldBe(3);
            });

            It($"filters {nameof(apps.NonPackageApps)} by the specified environment", () =>
            {
                apps.NonPackageApps.ShouldAllBe(x => x.Environments != excludedEnvironment);
                apps.NonPackageApps.Count.ShouldBe(3);
            });

            It($"filters {nameof(apps.PowerShellAppPackages)} by the specified environment", () =>
            {
                apps.PowerShellAppPackages.ShouldAllBe(x => x.Environments != excludedEnvironment);
                apps.PowerShellAppPackages.Count.ShouldBe(3);
            });
        }

        [Fact]
        public async Task When_loading_for_all_environments()
        {
            var appsPath = RandomString();
            var appsJson = RandomString();
            var specifiedEnvironment = new List<string> {"All"};

            GetMock<IArguments>().SetupGet(x => x.AppsPath).Returns(appsPath);
            GetMock<IArguments>().SetupGet(x => x.Environments).Returns(specifiedEnvironment);
            GetMock<IFileSystem>().Setup(x => x.ReadAllTextAsync(appsPath)).ReturnsAsync(appsJson);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<Configurator.Apps.Apps>(appsJson)).Returns(loadedApps);

            var apps = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It($"includes all {nameof(apps.WingetApps)}", () =>
            {
                apps.WingetApps.Count.ShouldBe(4);
            });

            It($"includes all {nameof(apps.ScoopApps)}", () =>
            {
                apps.ScoopApps.Count.ShouldBe(4);
            });

            It($"includes all {nameof(apps.NonPackageApps)}", () =>
            {
                apps.NonPackageApps.Count.ShouldBe(4);
            });

            It($"includes all {nameof(apps.PowerShellAppPackages)}", () =>
            {
                apps.PowerShellAppPackages.Count.ShouldBe(4);
            });
        }
    }
}
