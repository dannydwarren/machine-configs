﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMoqCore;
using Configurator.Apps;
using Configurator.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Xunit;

namespace Configurator.IntegrationTests
{
    public class ManifestRepositoryV2Tests : IntegrationTestBase<ManifestRepositoryV2>
    {
        private readonly Mock<IArguments> mockArgs;

        public ManifestRepositoryV2Tests()
        {
            var mocker = new AutoMoqer();
            mockArgs = mocker.GetMock<IArguments>();
            mockArgs.SetupGet(x => x.Environments).Returns(new List<string> {"Test"});
        }

        [Fact]
        public async Task When_parsing_scoop_apps()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/scoop-only_manifest_v2.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses", () =>
            {
                manifest.Apps.ShouldHaveSingleItem()
                    .ShouldBeOfType<ScoopApp>().AppId.ShouldBe("scoop-app-id");
            });
        }

        [Fact]
        public async Task When_parsing_scoop_buckets()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/scoop-buckets-only_manifest_v2.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses", () =>
            {
                manifest.Apps.ShouldHaveSingleItem()
                    .ShouldBeOfType<ScoopBucketApp>().AppId.ShouldBe("scoop-bucket-app-id");
            });
        }

        [Fact]
        public async Task When_parsing_winget_apps()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/winget-only_manifest_v2.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses", () =>
            {
                manifest.Apps.ShouldHaveSingleItem()
                    .ShouldBeOfType<WingetApp>().AppId.ShouldBe("winget-app-id");
            });
        }

        [Fact]
        public async Task When_parsing_non_package_apps()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/non-package-only_manifest_v2.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses", () =>
            {
                manifest.Apps.ShouldHaveSingleItem()
                    .ShouldBeOfType<NonPackageApp>().AppId.ShouldBe("non-package-app-id");
            });
        }

        [Fact]
        public async Task When_parsing_gitconfigs()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/gitconfigs-only_manifest_v2.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses", () =>
            {
                manifest.Apps.ShouldHaveSingleItem()
                    .ShouldBeOfType<GitconfigApp>().AppId.ShouldBe("gitconfig-app-id");
            });
        }

        [Fact]
        public async Task When_parsing_power_shell_app_packages()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/power-shell-app-package-only_manifest_v2.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses", () =>
            {
                manifest.Apps.ShouldHaveSingleItem()
                    .ShouldBeOfType<PowerShellAppPackage>().ShouldSatisfyAllConditions(x =>
                {
                    x.AppId.ShouldBe("power-shell-app-package-app-id");
                    x.Downloader.ShouldBe("some-downloader");
                    x.DownloaderArgs.ToString().ShouldNotBeEmpty();
                });
            });
        }

        [Fact]
        public async Task When_parsing_scripts()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/script-only_manifest_v2.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses", () =>
            {
                manifest.Apps.ShouldHaveSingleItem()
                    .ShouldBeOfType<ScriptApp>().ShouldSatisfyAllConditions(x =>
                    {
                        x.AppId.ShouldBe("script-app-id");
                        x.InstallScript.ShouldBe("install-script");
                        x.VerificationScript.ShouldBe("verification-script");
                    });
            });
        }

    }
}