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
    public class ManifestRepositoryTests : UnitTestBase<ManifestRepository>
    {
        private readonly ManifestRepository.RawManifest loadedRawRawManifest;
        private readonly List<ManifestRepository.Installable> installables;
        private readonly List<ScriptApp> knownScriptApps;

        public ManifestRepositoryTests()
        {
            loadedRawRawManifest = new ManifestRepository.RawManifest
            {
               Apps = new List<JsonElement>
               {
                   JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{""installable"": 1}"))).RootElement,
                   JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{""installable"": 2}"))).RootElement,
                   JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{""installable"": 3}"))).RootElement,
                   JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{""installable"": 4}"))).RootElement,
                   JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{""installable"": 5}"))).RootElement,
               }
            };

            installables = new List<ManifestRepository.Installable>
            {
                new ManifestRepository.Installable
                {
                    AppId = RandomString(),
                    AppType = AppType.Script,
                    Environments = "Personal".ToLower(),
                    AppData = JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{""app"": 1}"))).RootElement
                },
                new ManifestRepository.Installable
                {
                    AppId = RandomString(),
                    AppType = AppType.Script,
                    Environments = "Media",
                    AppData = JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{""app"": 2}"))).RootElement
                },
                new ManifestRepository.Installable
                {
                    AppId = RandomString(),
                    AppType = AppType.Script,
                    Environments = "Work",
                    AppData = JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{""app"": 3}"))).RootElement
                },
                new ManifestRepository.Installable
                {
                    AppId = RandomString(),
                    AppType = AppType.Script,
                    Environments = "All",
                    AppData = JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{""app"": 4}"))).RootElement
                },
                new ManifestRepository.Installable
                {
                    AppId = RandomString(),
                    AppType = AppType.Unknown,
                    Environments = "All",
                    AppData = JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{""app"": 5}"))).RootElement
                }
            };

            knownScriptApps = new List<ScriptApp>
            {
                new ScriptApp { AppId = installables[0].AppId },
                new ScriptApp { AppId = installables[1].AppId },
                new ScriptApp { AppId = installables[2].AppId },
                new ScriptApp { AppId = installables[3].AppId },
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
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.RawManifest>(manifestJson)).Returns(loadedRawRawManifest);

            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.Installable>(loadedRawRawManifest.Apps[0].ToString()!)).Returns(installables[0]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.Installable>(loadedRawRawManifest.Apps[1].ToString()!)).Returns(installables[1]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.Installable>(loadedRawRawManifest.Apps[2].ToString()!)).Returns(installables[2]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.Installable>(loadedRawRawManifest.Apps[3].ToString()!)).Returns(installables[3]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.Installable>(loadedRawRawManifest.Apps[4].ToString()!)).Returns(installables[4]);

            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ScriptApp>(installables[0].AppData.ToString()!)).Returns(knownScriptApps[0]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ScriptApp>(installables[2].AppData.ToString()!)).Returns(knownScriptApps[2]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ScriptApp>(installables[3].AppData.ToString()!)).Returns(knownScriptApps[3]);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("filters known apps by the specified environment", () =>
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
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.RawManifest>(manifestJson)).Returns(loadedRawRawManifest);

            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.Installable>(loadedRawRawManifest.Apps[0].ToString()!)).Returns(installables[0]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.Installable>(loadedRawRawManifest.Apps[1].ToString()!)).Returns(installables[1]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.Installable>(loadedRawRawManifest.Apps[2].ToString()!)).Returns(installables[2]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.Installable>(loadedRawRawManifest.Apps[3].ToString()!)).Returns(installables[3]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.Installable>(loadedRawRawManifest.Apps[4].ToString()!)).Returns(installables[4]);

            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ScriptApp>(installables[0].AppData.ToString()!)).Returns(knownScriptApps[0]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ScriptApp>(installables[1].AppData.ToString()!)).Returns(knownScriptApps[1]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ScriptApp>(installables[2].AppData.ToString()!)).Returns(knownScriptApps[2]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ScriptApp>(installables[3].AppData.ToString()!)).Returns(knownScriptApps[3]);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("includes all known apps", () => { manifest.Apps.Count.ShouldBe(4); });

            It("excludes unknown apps", () =>
            {
                GetMock<IJsonSerializer>().VerifyNever(x => x.Deserialize<ScriptApp>(installables[4].AppData.ToString()!));
            });
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
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.RawManifest>(manifestJson)).Returns(loadedRawRawManifest);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.Installable>(IsAny<string>())).Returns(installables[0]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ScriptApp>(IsAny<string>())).Returns(knownScriptApps[0]);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("builds the manifest", () => { manifest.ShouldNotBeNull().Apps.ShouldNotBeEmpty(); });
        }

        [Fact]
        public async Task When_loading_a_single_app()
        {
            var manifestPath = RandomString();
            var manifestJson = RandomString();
            var specifiedEnvironment = new List<string> { "Personal", "Work" };
            var expectedSingleAppId = knownScriptApps.Last().AppId;

            GetMock<IArguments>().SetupGet(x => x.ManifestPath).Returns(manifestPath);
            GetMock<IArguments>().SetupGet(x => x.Environments).Returns(specifiedEnvironment);
            GetMock<IArguments>().SetupGet(x => x.SingleAppId).Returns(expectedSingleAppId);

            GetMock<IFileSystem>().Setup(x => x.ReadAllTextAsync(manifestPath)).ReturnsAsync(manifestJson);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.RawManifest>(manifestJson)).Returns(loadedRawRawManifest);

            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.Installable>(loadedRawRawManifest.Apps[0].ToString()!)).Returns(installables[0]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.Installable>(loadedRawRawManifest.Apps[1].ToString()!)).Returns(installables[1]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.Installable>(loadedRawRawManifest.Apps[2].ToString()!)).Returns(installables[2]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.Installable>(loadedRawRawManifest.Apps[3].ToString()!)).Returns(installables[3]);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ManifestRepository.Installable>(loadedRawRawManifest.Apps[4].ToString()!)).Returns(installables[4]);

            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<ScriptApp>(installables[3].AppData.ToString()!)).Returns(knownScriptApps[3]);

            var manifest = await BecauseAsync(() => ClassUnderTest.LoadAsync());

            It("only includes the single app in the manifest", () =>
            {
                GetMock<IJsonSerializer>().Verify(x => x.Deserialize<ScriptApp>(IsAny<string>()), Times.Once);

                manifest.Apps.ShouldHaveSingleItem().AppId.ShouldBe(expectedSingleAppId);
            });
        }
    }
}
