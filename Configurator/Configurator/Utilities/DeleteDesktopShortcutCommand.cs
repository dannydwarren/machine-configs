using System;

namespace Configurator.Utilities
{
    public interface IDeleteDesktopShortcutCommand
    {
        void Execute(string shortcutName);
    }

    public class DeleteDesktopShortcutCommand : IDeleteDesktopShortcutCommand
    {
        private readonly IFileSystem fileSystem;

        public DeleteDesktopShortcutCommand(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public void Execute(string shortcutName)
        {
            fileSystem.Delete($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{shortcutName}.lnk");
            fileSystem.Delete($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\Desktop\\{shortcutName}.lnk");
        }
    }
}
