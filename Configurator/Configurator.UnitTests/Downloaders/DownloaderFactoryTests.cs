using System;
using Configurator.Downloaders;
using Shouldly;
using Xunit;

namespace Configurator.UnitTests.Downloaders
{
    public class DownloaderFactoryTests : UnitTestBase<DownloaderFactory>
    {
        [Fact]
        public void When_getting_a_downloader()
        {
            var downloaderType = typeof(GitHubAssetDownloader);
            var expectedDownloader = new GitHubAssetDownloader(null!,null!,null!);

            GetMock<IServiceProvider>().Setup(x => x.GetService(downloaderType)).Returns(expectedDownloader);

            var downloader = Because(() => ClassUnderTest.GetDownloader(downloaderType.Name));

            It("gets the downloader matching the name", () =>
            {
                GetMock<IServiceProvider>().Verify(x => x.GetService(downloaderType));
                downloader.ShouldBeOfType(downloaderType);
            });
        }

        [Fact]
        public void When_getting_a_downloader_and_cannot_find_the_downloader_type()
        {
            var unregisteredDownloaderName = RandomString();
            var exception = BecauseThrows<Exception>(() => ClassUnderTest.GetDownloader(unregisteredDownloaderName));

            It("throws with a useful message", () =>
            {
                GetMock<IServiceProvider>().VerifyNever(x => x.GetService(IsAny<Type>()));
                exception.ShouldNotBeNull().ShouldSatisfyAllConditions(x =>
                {
                   x.Message.ShouldContain(unregisteredDownloaderName);
                   x.Message.ShouldContain(typeof(IDownloader).Namespace!);
                });
            });
        }
    }
}
