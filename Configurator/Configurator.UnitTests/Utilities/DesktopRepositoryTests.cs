using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Configurator.Utilities;
using Shouldly;
using Xunit;

namespace Configurator.UnitTests.Utilities
{
    public class DesktopRepositoryTests : UnitTestBase<DesktopRepository>
    {
        [Fact]
        public void When_loading_system_entries()
        {
            var publicDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var userProfileDesktopPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Desktop");
            var publicDesktopSystemEntries = new List<string>
            {
                Path.Combine(publicDesktopPath, RandomString()),
                Path.Combine(publicDesktopPath, RandomString())
            };
            var userProfileDesktopSystemEntries =new List<string>
            {
                Path.Combine(userProfileDesktopPath, RandomString()),
                Path.Combine(userProfileDesktopPath, RandomString()),
                Path.Combine(userProfileDesktopPath, RandomString())
            };
            var allDesktopsSystemEntries = publicDesktopSystemEntries.Union(userProfileDesktopSystemEntries);

            GetMock<IFileSystem>().Setup(x => x.EnumerateFileSystemEntries(publicDesktopPath)).Returns(publicDesktopSystemEntries);
            GetMock<IFileSystem>().Setup(x => x.EnumerateFileSystemEntries(userProfileDesktopPath)).Returns(userProfileDesktopSystemEntries);

            var filenames = Because(() => ClassUnderTest.LoadSystemEntries());

            It("returns filenames", () =>
            {
                filenames.ShouldBe(allDesktopsSystemEntries);
            });
        }

        [Fact]
        public void When_deleting()
        {
            var paths = new List<string>
            {
                RandomString(),
                RandomString()
            };

            Because(() => ClassUnderTest.DeletePaths(paths));

            It("deletes each path", () =>
            {
                GetMock<IFileSystem>().Verify(x => x.Delete(paths[0]));
                GetMock<IFileSystem>().Verify(x => x.Delete(paths[1]));
            });
        }
    }
}
