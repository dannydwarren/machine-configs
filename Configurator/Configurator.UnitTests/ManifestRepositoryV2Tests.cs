using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.Configuration;
using Configurator.Utilities;
using Moq;
using Shouldly;
using Xunit;

namespace Configurator.UnitTests
{
    public class ManifestRepositoryV2Tests : UnitTestBase<ManifestRepositoryV2>
    {
        private readonly ManifestV2Raw loadedRawManifest;
        private readonly List<Installable> installables;
        private readonly ManifestV2 fullManifest;

        public ManifestRepositoryV2Tests()
        {
            loadedRawManifest = new ManifestV2Raw
            {
               Apps = new List<JsonElement>
               {
                   JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{""installable"": 1}"))).RootElement,
                   JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{""installable"": 2}"))).RootElement,
                   JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{""installable"": 3}"))).RootElement,
                   JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{""installable"": 4}"))).RootElement
               }
            };

            installables = new List<Installable>
            {
                new Installable
                {
                    Installer = Installer.Script,
                    Environments = "Personal".ToLower(),
                    AppData = JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{""app"": 1}"))).RootElement
                },
                new Installable
                {
                    Installer = Installer.Script,
                    Environments = "Media",
                    AppData = JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{""app"": 2}"))).RootElement
                },
                new Installable
                {
                    Installer = Installer.Script,
                    Environments = "Work",
                    AppData = JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{""app"": 3}"))).RootElement
                },
                new Installable
                {
                    Installer = Installer.Script,
                    Environments = "All",
                    AppData = JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{""app"": 4}"))).RootElement
                }
            };

            fullManifest = new ManifestV2
            {
                Apps = new List<IApp>
                {
                    new ScriptApp { AppId = RandomString() },
                    new ScriptApp { AppId = RandomString() },
                    new ScriptApp { AppId = RandomString() },
                    new ScriptApp { AppId = RandomString() }
                }
            };
        }

        [Fact]
        public async Task When_loading_with_a_specified_environment()
        {
            var manifestPath = RandomString();
            var manifestJson = RandomString();
            var specifiedEnvironment = new List<string> { "Personal", "Work" };

            GetMock<IArguments>().SetupGet(x => x.ManifestPath).Returns(manifestPath);
            GetMock<IArguments>().SetupGet(x => x.Environments).Returns(specifiedEnvironment);
            GetMock<IFileSystem>().Setup(x => x.ReadAllTextAsync(manifestPath)).ReturnsAsync(manifestJson);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestV2Raw>(manifestJson)).Returns(loadedRawManifest);

            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<Installable>(loadedRawManifest.Apps[0].ToString()!)).Returns(installables[0]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<Installable>(loadedRawManifest.Apps[1].ToString()!)).Returns(installables[1]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<Installable>(loadedRawManifest.Apps[2].ToString()!)).Returns(installables[2]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<Installable>(loadedRawManifest.Apps[3].ToString()!)).Returns(installables[3]);

            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ScriptApp>(installables[0].AppData.ToString()!)).Returns((ScriptApp)fullManifest.Apps[0]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ScriptApp>(installables[2].AppData.ToString()!)).Returns((ScriptApp)fullManifest.Apps[2]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ScriptApp>(installables[3].AppData.ToString()!)).Returns((ScriptApp)fullManifest.Apps[3]);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("filters apps by the specified environment", () =>
            {
                GetMock<IJsonSerializer>().VerifyNever(x => x.Deserialize<ScriptApp>(installables[1].AppData.ToString()!));

                manifest.Apps.Count.ShouldBe(3);
            });
        }

        [Fact]
        public async Task When_loading_for_all_environments()
        {
            var manifestPath = RandomString();
            var manifestJson = RandomString();
            var specifiedEnvironment = new List<string> { "All" };

            GetMock<IArguments>().SetupGet(x => x.ManifestPath).Returns(manifestPath);
            GetMock<IArguments>().SetupGet(x => x.Environments).Returns(specifiedEnvironment);
            GetMock<IFileSystem>().Setup(x => x.ReadAllTextAsync(manifestPath)).ReturnsAsync(manifestJson);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestV2Raw>(manifestJson)).Returns(loadedRawManifest);

            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<Installable>(loadedRawManifest.Apps[0].ToString()!)).Returns(installables[0]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<Installable>(loadedRawManifest.Apps[1].ToString()!)).Returns(installables[1]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<Installable>(loadedRawManifest.Apps[2].ToString()!)).Returns(installables[2]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<Installable>(loadedRawManifest.Apps[3].ToString()!)).Returns(installables[3]);

            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ScriptApp>(installables[0].AppData.ToString()!)).Returns((ScriptApp)fullManifest.Apps[0]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ScriptApp>(installables[1].AppData.ToString()!)).Returns((ScriptApp)fullManifest.Apps[1]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ScriptApp>(installables[2].AppData.ToString()!)).Returns((ScriptApp)fullManifest.Apps[2]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ScriptApp>(installables[3].AppData.ToString()!)).Returns((ScriptApp)fullManifest.Apps[3]);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("includes all apps", () => { manifest.Apps.Count.ShouldBe(4); });
        }

        [Fact]
        public async Task When_loading_from_http_resource()
        {
            var manifestFilename = $"{RandomString()}.json";
            var httpManifestPath = @$"http://url-only/{manifestFilename}";
            var downloadedManifestPath = RandomString();
            var manifestJson = RandomString();
            var specifiedEnvironment = new List<string> { "All" };

            GetMock<IArguments>().SetupGet(x => x.ManifestPath).Returns(httpManifestPath);
            GetMock<IArguments>().SetupGet(x => x.Environments).Returns(specifiedEnvironment);
            GetMock<IResourceDownloader>().Setup(x => x.DownloadAsync(httpManifestPath, manifestFilename))
                .ReturnsAsync(downloadedManifestPath);
            GetMock<IFileSystem>().Setup(x => x.ReadAllTextAsync(downloadedManifestPath)).ReturnsAsync(manifestJson);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestV2Raw>(manifestJson)).Returns(loadedRawManifest);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<Installable>(IsAny<string>())).Returns(installables[0]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ScriptApp>(IsAny<string>())).Returns((ScriptApp)fullManifest.Apps[0]);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("builds the manifest", () => { manifest.ShouldNotBeNull().Apps.ShouldNotBeEmpty(); });
        }
    }
}
