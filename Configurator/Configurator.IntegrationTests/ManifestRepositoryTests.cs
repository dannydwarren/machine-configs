using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMoqCore;
using Configurator.Apps;
using Configurator.Utilities;
using Configurator.Windows;
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
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/scoop-only_manifest.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses required properties", () =>
            {
                manifest.Apps[0]
                    .ShouldBeOfType<ScoopApp>().ShouldSatisfyAllConditions(x =>
                    {
                        x.AppId.ShouldBe("scoop-app-id");
                        x.InstallArgs.ShouldBeEmpty();
                        x.PreventUpgrade.ShouldBeFalse();
                        x.Configuration.ShouldBeNull();
                    });
            });

            It($"parses with {nameof(ScoopApp.InstallArgs)}", () =>
            {
                manifest.Apps[1]
                    .ShouldBeOfType<ScoopApp>().ShouldSatisfyAllConditions(x =>
                    {
                        x.AppId.ShouldBe("scoop-app-id-with-install-args");
                        x.InstallArgs.ShouldBe(" install-args");
                    });
            });

            It($"parses with {nameof(ScoopApp.PreventUpgrade)}", () =>
            {
                manifest.Apps[2]
                    .ShouldBeOfType<ScoopApp>().ShouldSatisfyAllConditions(x =>
                    {
                        x.AppId.ShouldBe("scoop-app-id-with-prevent-upgrade");
                        x.PreventUpgrade.ShouldBeTrue();
                    });
            });

            It($"parses with {nameof(ScoopApp.Configuration)}", () =>
            {
                manifest.Apps[3]
                    .ShouldBeOfType<ScoopApp>().ShouldSatisfyAllConditions(x =>
                    {
                        x.AppId.ShouldBe("scoop-app-id-with-configuration");
                        x.Configuration.RegistrySettings.ShouldHaveSingleItem()
                            .ShouldSatisfyAllConditions(y =>
                            {
                                y.KeyName.ShouldBe("key-name-test");
                                y.ValueName.ShouldBe("value-name-test");
                                y.ValueData.ShouldBe("value-data-test");
                            });
                    });
            });
        }

        [Fact]
        public async Task When_parsing_scoop_buckets()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/scoop-buckets-only_manifest.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses required properties", () =>
            {
                manifest.Apps.ShouldHaveSingleItem()
                    .ShouldBeOfType<ScoopBucketApp>().AppId.ShouldBe("scoop-bucket-app-id");
            });
        }

        [Fact]
        public async Task When_parsing_winget_apps()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/winget-only_manifest.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses required properties", () =>
            {
                manifest.Apps[0]
                    .ShouldBeOfType<WingetApp>().ShouldSatisfyAllConditions(x =>
                    {
                        x.AppId.ShouldBe("winget-app-id");
                        x.InstallArgs.ShouldBeEmpty();
                        x.PreventUpgrade.ShouldBeFalse();
                        x.Configuration.ShouldBeNull();
                    });
            });

            It($"parses with {nameof(WingetApp.InstallArgs)}", () =>
            {
                manifest.Apps[1]
                    .ShouldBeOfType<WingetApp>().ShouldSatisfyAllConditions(x =>
                    {
                        x.AppId.ShouldBe("winget-app-id-with-install-args");
                        x.InstallArgs.ShouldBe(" --override install-args");
                    });
            });

            It($"parses with {nameof(WingetApp.PreventUpgrade)}", () =>
            {
                manifest.Apps[2]
                    .ShouldBeOfType<WingetApp>().ShouldSatisfyAllConditions(x =>
                    {
                        x.AppId.ShouldBe("winget-app-id-with-prevent-upgrade");
                        x.PreventUpgrade.ShouldBeTrue();
                    });
            });

            It($"parses with {nameof(WingetApp.Configuration)}", () =>
            {
                manifest.Apps[3]
                    .ShouldBeOfType<WingetApp>().ShouldSatisfyAllConditions(x =>
                    {
                        x.AppId.ShouldBe("winget-app-id-with-configuration");
                        x.Configuration.RegistrySettings.ShouldHaveSingleItem()
                            .ShouldSatisfyAllConditions(y =>
                            {
                                y.KeyName.ShouldBe("key-name-test");
                                y.ValueName.ShouldBe("value-name-test");
                                y.ValueData.ShouldBe("value-data-test");
                            });
                    });
            });
        }

        [Fact]
        public async Task When_parsing_power_shell_module_apps()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/power-shell-module-only_manifest.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses required properties", () =>
            {
                manifest.Apps[0]
                    .ShouldBeOfType<PowerShellModuleApp>().ShouldSatisfyAllConditions(x =>
                    {
                        x.AppId.ShouldBe("power-shell-module-app-id");
                        x.InstallArgs.ShouldBeEmpty();
                        x.PreventUpgrade.ShouldBeFalse();
                        x.Configuration.ShouldBeNull();
                    });
            });

            It($"parses with {nameof(PowerShellModuleApp.InstallArgs)}", () =>
            {
                manifest.Apps[1]
                    .ShouldBeOfType<PowerShellModuleApp>().ShouldSatisfyAllConditions(x =>
                    {
                        x.AppId.ShouldBe("power-shell-module-app-id-with-install-args");
                        x.InstallArgs.ShouldBe(" install-args");
                    });
            });

            It($"parses with {nameof(PowerShellModuleApp.PreventUpgrade)}", () =>
            {
                manifest.Apps[2]
                    .ShouldBeOfType<PowerShellModuleApp>().ShouldSatisfyAllConditions(x =>
                    {
                        x.AppId.ShouldBe("power-shell-module-app-id-with-prevent-upgrade");
                        x.PreventUpgrade.ShouldBeTrue();
                    });
            });

            It($"parses with {nameof(PowerShellModuleApp.Configuration)}", () =>
            {
                manifest.Apps[3]
                    .ShouldBeOfType<PowerShellModuleApp>().ShouldSatisfyAllConditions(x =>
                    {
                        x.AppId.ShouldBe("power-shell-module-app-id-with-configuration");
                        x.Configuration.RegistrySettings.ShouldHaveSingleItem()
                            .ShouldSatisfyAllConditions(y =>
                            {
                                y.KeyName.ShouldBe("key-name-test");
                                y.ValueName.ShouldBe("value-name-test");
                                y.ValueData.ShouldBe("value-data-test");
                            });
                    });
            });
        }

        [Fact]
        public async Task When_parsing_non_package_apps()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/non-package-only_manifest.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses required properties", () =>
            {
                manifest.Apps.ShouldHaveSingleItem()
                    .ShouldBeOfType<NonPackageApp>().AppId.ShouldBe("non-package-app-id");
            });
        }

        [Fact]
        public async Task When_parsing_gitconfigs()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/gitconfigs-only_manifest.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses required properties", () =>
            {
                manifest.Apps.ShouldHaveSingleItem()
                    .ShouldBeOfType<GitconfigApp>().AppId.ShouldBe("gitconfig-app-id");
            });
        }

        [Fact]
        public async Task When_parsing_power_shell_app_packages()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/power-shell-app-package-only_manifest.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses required properties", () =>
            {
                manifest.Apps.First()
                    .ShouldBeOfType<PowerShellAppPackage>().ShouldSatisfyAllConditions(x =>
                {
                    x.AppId.ShouldBe("power-shell-app-package-app-id");
                    x.Downloader.ShouldBe("some-downloader");
                    x.DownloaderArgs.ToString().ShouldNotBeEmpty();
                    x.PreventUpgrade.ShouldBeFalse();
                });
            });

            It($"parses with {nameof(PowerShellAppPackage.PreventUpgrade)}", () =>
            {
                manifest.Apps.Last()
                    .ShouldBeOfType<PowerShellAppPackage>().ShouldSatisfyAllConditions(x =>
                {
                    x.AppId.ShouldBe("power-shell-app-package-app-id-with-prevent-upgrade");
                    x.Downloader.ShouldBe("some-downloader");
                    x.DownloaderArgs.ToString().ShouldNotBeEmpty();
                    x.PreventUpgrade.ShouldBe(true);
                });
            });
        }

        [Fact]
        public async Task When_parsing_scripts()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/script-only_manifest.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses required properties", () =>
            {
                manifest.Apps[0]
                    .ShouldBeOfType<ScriptApp>().ShouldSatisfyAllConditions(x =>
                    {
                        x.AppId.ShouldBe("script-app-id");
                        x.InstallScript.ShouldBe("install-script");
                        x.VerificationScript.ShouldBe("verification-script");
                        x.UpgradeScript.ShouldBe("upgrade-script");
                        x.Configuration.ShouldBeNull();
                    });
            });

            It($"parses with {nameof(ScriptApp.Configuration)}", () =>
            {
                manifest.Apps[1]
                    .ShouldBeOfType<ScriptApp>().ShouldSatisfyAllConditions(x =>
                    {
                        x.AppId.ShouldBe("script-app-id-with-configuration");
                        x.Configuration.ShouldNotBeNull().RegistrySettings.ShouldHaveSingleItem()
                            .ShouldSatisfyAllConditions(y =>
                            {
                                y.KeyName.ShouldBe("key-name-test");
                                y.ValueName.ShouldBe("value-name-test");
                                y.ValueData.ShouldBe("value-data-test");
                            });
                    });
            });
        }

        [Fact]
        public async Task When_parsing_registry_settings()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/registry-settings_manifest.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            var registrySettings = manifest.Apps.Single().Configuration!.RegistrySettings;

            It($"parses string {nameof(RegistrySetting.ValueData)}", () =>
            {
                registrySettings[0].ShouldSatisfyAllConditions(x =>
                    {
                        x.KeyName.ShouldBe("key-1");
                        x.ValueName.ShouldBe("string");
                        x.ValueData.ShouldBe("string-data");
                    });
            });

            It($"parses string {nameof(RegistrySetting.ValueData)} with environment tokens", () =>
            {
                registrySettings[1].ShouldSatisfyAllConditions(x =>
                    {
                        x.KeyName.ShouldBe("key-2");
                        x.ValueName.ShouldBe("string");
                        x.ValueData.ShouldBe("C:\\Program Files\\string-data");
                    });
            });

            It($"parses int {nameof(RegistrySetting.ValueData)}", () =>
            {
                registrySettings[2].ShouldSatisfyAllConditions(x =>
                    {
                        x.KeyName.ShouldBe("key-3");
                        x.ValueName.ShouldBe("uint");
                        x.ValueData.ShouldBeOfType<uint>().ShouldBe((uint)42);
                    });
            });
        }

        [Fact]
        public async Task When_parsing_visual_studio_extension_apps()
        {
            mockArgs.SetupGet(x => x.ManifestPath).Returns("./TestManifests/visual-studio-extension-only_manifest.json");

            Services.AddTransient(_ => mockArgs.Object);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("parses required properties", () =>
            {
                manifest.Apps[0]
                    .ShouldBeOfType<VisualStudioExtensionApp>().ShouldSatisfyAllConditions(x =>
                    {
                        x.AppId.ShouldBe("visual-studio-extension-app-id");
                        x.DownloaderArgs.GetProperty("Publisher").GetString().ShouldBe("publisher-1");
                        x.DownloaderArgs.GetProperty("ExtensionName").GetString().ShouldBe("extension-name-1");
                    });
            });
        }
    }
}
