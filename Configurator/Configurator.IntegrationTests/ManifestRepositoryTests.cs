using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMoqCore;
using Configurator.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Xunit;

namespace Configurator.IntegrationTests
{
    public class ManifestRepositoryTests : IntegrationTestBase<ManifestRepository>
    {
        private readonly Mock<IArguments> mockArgs;

        public ManifestRepositoryTests()
        {
            var mocker = new AutoMoqer();
            mockArgs = mocker.GetMock<IArguments>();
            mockArgs.SetupGet(x => x.Environments).Returns(new List<string> {"Test"});
        }

        [Fact]
        public async Task When_parsing_scoop_apps()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/manifest_scoop_only.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses", () =>
            {
                manifest.ScoopApps.ShouldHaveSingleItem().AppId.ShouldBe("scoop-app-id");
            });
        }

        [Fact]
        public async Task When_parsing_winget_apps()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/manifest_winget_only.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses", () =>
            {
                manifest.WingetApps.ShouldHaveSingleItem().AppId.ShouldBe("winget-app-id");
            });
        }

        [Fact]
        public async Task When_parsing_non_package_apps()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/manifest_non-package_only.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses", () =>
            {
                manifest.NonPackageApps.ShouldHaveSingleItem().AppId.ShouldBe("non-package-app-id");
            });
        }

        [Fact]
        public async Task When_parsing_gitconfigs()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/manifest_gitconfigs_only.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses", () =>
            {
                manifest.Gitconfigs.ShouldHaveSingleItem().AppId.ShouldBe("gitconfig-app-id");
            });
        }

        [Fact]
        public async Task When_parsing_power_shell_app_packages()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/manifest_power-shell-app-package_only.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses", () =>
            {
                manifest.PowerShellAppPackages.ShouldHaveSingleItem().ShouldSatisfyAllConditions(x =>
                {
                    x.AppId.ShouldBe("power-shell-app-package-app-id");
                    x.Downloader.ShouldBe("some-downloader");
                    x.DownloaderArgs.ToString().ShouldNotBeEmpty();
                });
            });
        }
    }
}
