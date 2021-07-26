using System.Collections.Generic;
using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.Configuration;
using Configurator.Utilities;
using Moq;
using Shouldly;
using Xunit;

namespace Configurator.UnitTests
{
    public class ManifestRepositoryTests : UnitTestBase<ManifestRepository>
    {
        private readonly Manifest loadedManifest;

        public ManifestRepositoryTests()
        {
            loadedManifest = new Manifest
            {
                WingetApps = new List<WingetApp>
                {
                    new WingetApp {AppId = RandomString(), Environments = "Personal".ToLower()},
                    new WingetApp {AppId = RandomString(), Environments = "Media"},
                    new WingetApp {AppId = RandomString(), Environments = "Work"},
                    new WingetApp {AppId = RandomString(), Environments = "All"}
                },
                ScoopApps = new List<ScoopApp>
                {
                    new ScoopApp {AppId = RandomString(), Environments = "Personal"},
                    new ScoopApp {AppId = RandomString(), Environments = "Media"},
                    new ScoopApp {AppId = RandomString(), Environments = "Work"},
                    new ScoopApp {AppId = RandomString(), Environments = "All"}
                },
                NonPackageApps = new List<NonPackageApp>
                {
                    new NonPackageApp {AppId = RandomString(), Environments = "Personal"},
                    new NonPackageApp {AppId = RandomString(), Environments = "Media"},
                    new NonPackageApp {AppId = RandomString(), Environments = "Work"},
                    new NonPackageApp {AppId = RandomString(), Environments = "All"}
                },
                PowerShellAppPackages = new List<PowerShellAppPackage>
                {
                    new PowerShellAppPackage {AppId = RandomString(), Environments = "Personal"},
                    new PowerShellAppPackage {AppId = RandomString(), Environments = "Media"},
                    new PowerShellAppPackage {AppId = RandomString(), Environments = "Work"},
                    new PowerShellAppPackage {AppId = RandomString(), Environments = "All"}
                },
                Gitconfigs = new List<GitconfigApp>
                {
                    new GitconfigApp {AppId = RandomString(), Environments = "Personal"},
                    new GitconfigApp {AppId = RandomString(), Environments = "Media"},
                    new GitconfigApp {AppId = RandomString(), Environments = "Work"},
                    new GitconfigApp {AppId = RandomString(), Environments = "All"}
                },
            };
        }

        [Fact]
        public async Task When_loading()
        {
            var manifestPath = RandomString();
            var manifestJson = RandomString();
            var specifiedEnvironment = new List<string> {"Personal", "Work"};
            var excludedEnvironment = "Media";

            GetMock<IArguments>().SetupGet(x => x.ManifestPath).Returns(manifestPath);
            GetMock<IArguments>().SetupGet(x => x.Environments).Returns(specifiedEnvironment);
            GetMock<IFileSystem>().Setup(x => x.ReadAllTextAsync(manifestPath)).ReturnsAsync(manifestJson);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<Manifest>(manifestJson)).Returns(loadedManifest);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It($"filters {nameof(manifest.WingetApps)} by the specified environment", () =>
            {
                manifest.WingetApps.ShouldAllBe(x => x.Environments != excludedEnvironment);
                manifest.WingetApps.Count.ShouldBe(3);
            });

            It($"filters {nameof(manifest.ScoopApps)} by the specified environment", () =>
            {
                manifest.ScoopApps.ShouldAllBe(x => x.Environments != excludedEnvironment);
                manifest.ScoopApps.Count.ShouldBe(3);
            });

            It($"filters {nameof(manifest.NonPackageApps)} by the specified environment", () =>
            {
                manifest.NonPackageApps.ShouldAllBe(x => x.Environments != excludedEnvironment);
                manifest.NonPackageApps.Count.ShouldBe(3);
            });

            It($"filters {nameof(manifest.PowerShellAppPackages)} by the specified environment", () =>
            {
                manifest.PowerShellAppPackages.ShouldAllBe(x => x.Environments != excludedEnvironment);
                manifest.PowerShellAppPackages.Count.ShouldBe(3);
            });

            It($"filters {nameof(manifest.Gitconfigs)} by the specified environment", () =>
            {
                manifest.Gitconfigs.ShouldAllBe(x => x.Environments != excludedEnvironment);
                manifest.Gitconfigs.Count.ShouldBe(3);
            });
        }

        [Fact]
        public async Task When_loading_for_all_environments()
        {
            var manifestPath = RandomString();
            var manifestJson = RandomString();
            var specifiedEnvironment = new List<string> {"All"};

            GetMock<IArguments>().SetupGet(x => x.ManifestPath).Returns(manifestPath);
            GetMock<IArguments>().SetupGet(x => x.Environments).Returns(specifiedEnvironment);
            GetMock<IFileSystem>().Setup(x => x.ReadAllTextAsync(manifestPath)).ReturnsAsync(manifestJson);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<Manifest>(manifestJson)).Returns(loadedManifest);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It($"includes all {nameof(manifest.WingetApps)}", () =>
            {
                manifest.WingetApps.Count.ShouldBe(4);
            });

            It($"includes all {nameof(manifest.ScoopApps)}", () =>
            {
                manifest.ScoopApps.Count.ShouldBe(4);
            });

            It($"includes all {nameof(manifest.NonPackageApps)}", () =>
            {
                manifest.NonPackageApps.Count.ShouldBe(4);
            });

            It($"includes all {nameof(manifest.PowerShellAppPackages)}", () =>
            {
                manifest.PowerShellAppPackages.Count.ShouldBe(4);
            });

            It($"includes all {nameof(manifest.Gitconfigs)}", () =>
            {
                manifest.Gitconfigs.Count.ShouldBe(4);
            });
        }
    }
}
