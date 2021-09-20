using System;
using Configurator.PowerShell;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.Installers;
using Configurator.Utilities;
using Xunit;

namespace Configurator.UnitTests
{
    public class MachineConfiguratorTests : UnitTestBase<MachineConfigurator>
    {
        [Fact]
        public async Task When_executing()
        {
            var manifest = new Manifest
            {
                Apps = new List<IApp>
                {
                    new PowerShellAppPackage { AppId = RandomString() },
                    new ScriptApp { AppId = RandomString() },
                    new NonPackageApp { AppId = RandomString() },
                    new ScoopBucketApp { AppId = RandomString() },
                    new ScoopApp { AppId = RandomString() },
                    new WingetApp { AppId = RandomString() },
                    new GitconfigApp { AppId = RandomString() },
                    new WingetApp { AppId = RandomString() },
                }
            };

            GetMock<IManifestRepository>().Setup(x => x.LoadAsync()).ReturnsAsync(manifest);

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync());

            It("initializes the system", () => { GetMock<ISystemInitializer>().Verify(x => x.InitializeAsync()); });

            It("installs all apps", () =>
            {
                GetMock<IDownloadAppInstaller>().Verify(x => x.InstallAsync((IDownloadApp)manifest.Apps[0]));
                GetMock<IAppInstaller>().Verify(x => x.InstallOrUpgradeAsync(manifest.Apps[1]));
                GetMock<IAppInstaller>().Verify(x => x.InstallOrUpgradeAsync(manifest.Apps[2]));
                GetMock<IAppInstaller>().Verify(x => x.InstallOrUpgradeAsync(manifest.Apps[3]));
                GetMock<IAppInstaller>().Verify(x => x.InstallOrUpgradeAsync(manifest.Apps[4]));
                GetMock<IAppInstaller>().Verify(x => x.InstallOrUpgradeAsync(manifest.Apps[5]));
                GetMock<IAppInstaller>().Verify(x => x.InstallOrUpgradeAsync(manifest.Apps[6]));
                GetMock<IAppInstaller>().Verify(x => x.InstallOrUpgradeAsync(manifest.Apps[7]));
            });

            It("configures all apps", () =>
            {
                GetMock<IAppConfigurator>().Verify(x => x.Configure(manifest.Apps[0]));
                GetMock<IAppConfigurator>().Verify(x => x.Configure(manifest.Apps[1]));
                GetMock<IAppConfigurator>().Verify(x => x.Configure(manifest.Apps[2]));
                GetMock<IAppConfigurator>().Verify(x => x.Configure(manifest.Apps[3]));
                GetMock<IAppConfigurator>().Verify(x => x.Configure(manifest.Apps[4]));
                GetMock<IAppConfigurator>().Verify(x => x.Configure(manifest.Apps[5]));
                GetMock<IAppConfigurator>().Verify(x => x.Configure(manifest.Apps[6]));
                GetMock<IAppConfigurator>().Verify(x => x.Configure(manifest.Apps[7]));
            });
        }

        [Fact]
        public async Task When_an_exception_occurs()
        {
            var exception = new Exception(RandomString());

            GetMock<IPowerShellConfiguration>().Setup(x => x.SetExecutionPolicyAsync()).Throws(exception);

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync());

            It("logs the exception as an error",
                () => { GetMock<IConsoleLogger>().Setup(x => x.Error(exception.ToString())); });
        }
    }
}
