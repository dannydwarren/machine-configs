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
            var desktopPaths = new List<string> { RandomString(), RandomString() };

            GetMock<ISpecialFolders>().Setup(x => x.GetDesktopPaths()).Returns(desktopPaths);

            var userDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var commonDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
            var userDesktopSystemEntries = new List<string>
            {
                Path.Combine(userDesktopPath, RandomString()),
                Path.Combine(userDesktopPath, RandomString())
            };
            var commonDesktopSystemEntries =new List<string>
            {
                Path.Combine(commonDesktopPath, RandomString()),
                Path.Combine(commonDesktopPath, RandomString()),
                Path.Combine(commonDesktopPath, RandomString())
            };
            var allDesktopsSystemEntries = userDesktopSystemEntries.Union(commonDesktopSystemEntries);

            GetMock<IFileSystem>().Setup(x => x.EnumerateFileSystemEntries(desktopPaths[0])).Returns(userDesktopSystemEntries);
            GetMock<IFileSystem>().Setup(x => x.EnumerateFileSystemEntries(desktopPaths[1])).Returns(commonDesktopSystemEntries);

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
