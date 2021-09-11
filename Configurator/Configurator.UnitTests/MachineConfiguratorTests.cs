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
            var manifest = new ManifestV2
            {
                Apps = new List<IApp>
                {
                    new PowerShellAppPackage {AppId = RandomString(), Environments = "Personal"},
                    new ScriptApp {AppId = RandomString(), Environments = "All"},
                    new NonPackageApp {AppId = RandomString(), Environments = "All"},
                    new ScoopBucketApp {AppId = RandomString(), Environments = "All"},
                    new ScoopApp {AppId = RandomString(), Environments = "Personal"},
                    new WingetApp {AppId = RandomString(), Environments = "Personal"},
                    new GitconfigApp {AppId = RandomString(), Environments = "All"},
                    new WingetApp {AppId = RandomString(), Environments = "All"}
                }
            };

            GetMock<IManifestRepositoryV2>().Setup(x => x.LoadAsync()).ReturnsAsync(manifest);

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync());

            It("initializes the system", () =>
            {
                GetMock<ISystemInitializer>().Verify(x => x.InitializeAsync());
            });

            It("installs all apps", () =>
            {
                GetMock<IDownloadAppInstaller>().Verify(x => x.InstallAsync((IDownloadApp)manifest.Apps[0]));
                GetMock<IAppInstaller>().Verify(x => x.InstallAsync(manifest.Apps[1]));
                GetMock<IAppInstaller>().Verify(x => x.InstallAsync(manifest.Apps[2]));
                GetMock<IAppInstaller>().Verify(x => x.InstallAsync(manifest.Apps[3]));
                GetMock<IAppInstaller>().Verify(x => x.InstallAsync(manifest.Apps[4]));
                GetMock<IAppInstaller>().Verify(x => x.InstallAsync(manifest.Apps[5]));
                GetMock<IAppInstaller>().Verify(x => x.InstallAsync(manifest.Apps[6]));
                GetMock<IAppInstaller>().Verify(x => x.InstallAsync(manifest.Apps[7]));
            });
        }

        [Fact]
        public async Task When_an_exception_occurs()
        {
            var exception = new Exception(RandomString());

            GetMock<IPowerShellConfiguration>().Setup(x => x.SetExecutionPolicyAsync()).Throws(exception);

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync());

            It("logs the exception as an error", () =>
            {
                GetMock<IConsoleLogger>().Setup(x => x.Error(exception.ToString()));
            });
        }
    }
}
