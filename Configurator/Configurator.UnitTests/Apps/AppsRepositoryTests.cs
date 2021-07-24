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
        [Fact]
        public async Task When_loading()
        {
            var appsPath = RandomString();
            var appsJson = RandomString();
            var specifiedEnvironment = new List<string> {"Personal", "Work"};
            var excludedEnvironment = "Media";
            var expectedApps = new Configurator.Apps.Apps
            {
                WingetApps = new()
                {
                    new WingetApp {AppId = RandomString(), Environments = "Personal"},
                    new WingetApp {AppId = RandomString(), Environments = excludedEnvironment},
                    new WingetApp {AppId = RandomString(), Environments = "Work"}
                },
                ScoopApps = new()
                {
                    new ScoopApp {AppId = RandomString(), Environments = "Personal"},
                    new ScoopApp {AppId = RandomString(), Environments = excludedEnvironment},
                    new ScoopApp {AppId = RandomString(), Environments = "Work"}
                },
                NonPackageApps = new()
                {
                    new NonPackageApp {AppId = RandomString(), Environments = "Personal"},
                    new NonPackageApp {AppId = RandomString(), Environments = excludedEnvironment},
                    new NonPackageApp {AppId = RandomString(), Environments = "Work"}
                }
            };

            GetMock<IArguments>().SetupGet(x => x.AppsPath).Returns(appsPath);
            GetMock<IArguments>().SetupGet(x => x.Environments).Returns(specifiedEnvironment);
            GetMock<IFileSystem>().Setup(x => x.ReadAllTextAsync(appsPath)).ReturnsAsync(appsJson);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<Configurator.Apps.Apps>(appsJson)).Returns(expectedApps);

            var apps = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It($"filters {nameof(apps.WingetApps)} by the specified environment", () =>
            {
                apps.WingetApps.ShouldAllBe(x => x.Environments != excludedEnvironment);
                apps.WingetApps.Count.ShouldBe(2);
            });

            It($"filters {nameof(apps.ScoopApps)} by the specified environment", () =>
            {
                apps.ScoopApps.ShouldAllBe(x => x.Environments != excludedEnvironment);
                apps.ScoopApps.Count.ShouldBe(2);
            });

            It($"filters {nameof(apps.NonPackageApps)} by the specified environment", () =>
            {
                apps.NonPackageApps.ShouldAllBe(x => x.Environments != excludedEnvironment);
                apps.NonPackageApps.Count.ShouldBe(2);
            });
        }

        [Fact]
        public async Task When_loading_for_all_environments()
        {
            var appsPath = RandomString();
            var appsJson = RandomString();
            var specifiedEnvironment = new List<string> {"All"};
            var expectedApps = new Configurator.Apps.Apps
            {
                WingetApps = new()
                {
                    new WingetApp {AppId = RandomString(), Environments = "Personal"},
                    new WingetApp {AppId = RandomString(), Environments = "Media"},
                    new WingetApp {AppId = RandomString(), Environments = "All"}
                },
                ScoopApps = new()
                {
                    new ScoopApp {AppId = RandomString(), Environments = "Personal"},
                    new ScoopApp {AppId = RandomString(), Environments = "Media"},
                    new ScoopApp {AppId = RandomString(), Environments = "Work"}
                },
                NonPackageApps = new()
                {
                    new NonPackageApp {AppId = RandomString(), Environments = "Personal"},
                    new NonPackageApp {AppId = RandomString(), Environments = "Media"},
                    new NonPackageApp {AppId = RandomString(), Environments = "Work"}
                }
            };

            GetMock<IArguments>().SetupGet(x => x.AppsPath).Returns(appsPath);
            GetMock<IArguments>().SetupGet(x => x.Environments).Returns(specifiedEnvironment);
            GetMock<IFileSystem>().Setup(x => x.ReadAllTextAsync(appsPath)).ReturnsAsync(appsJson);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<Configurator.Apps.Apps>(appsJson)).Returns(expectedApps);

            var apps = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It($"includes all {nameof(apps.WingetApps)}", () =>
            {
                apps.WingetApps.Count.ShouldBe(3);
            });

            It($"includes all {nameof(apps.ScoopApps)}", () =>
            {
                apps.ScoopApps.Count.ShouldBe(3);
            });

            It($"includes all {nameof(apps.NonPackageApps)}", () =>
            {
                apps.NonPackageApps.Count.ShouldBe(3);
            });
        }
    }
}
