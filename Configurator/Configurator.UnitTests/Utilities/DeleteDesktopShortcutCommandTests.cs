using System;
using Configurator.Utilities;
using Xunit;

namespace Configurator.UnitTests.Utilities
{
    public class DeleteDesktopShortcutCommandTests : UnitTestBase<DeleteDesktopShortcutCommand>
    {
        [Fact]
        public void When_executing()
        {
            var shortcutName = RandomString();

            Because(() => ClassUnderTest.Execute(shortcutName));

            It("deletes the shortcut in the public desktop", () =>
            {
                GetMock<IFileSystem>().Verify(x => x.Delete($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{shortcutName}.lnk"));
            });

            It("deletes the shortcut in the user profile desktop", () =>
            {
                GetMock<IFileSystem>().Verify(x => x.Delete($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\Desktop\\{shortcutName}.lnk"));
            });
        }
    }
}
