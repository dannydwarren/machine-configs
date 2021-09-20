using Configurator.Apps;
using Configurator.Windows;

namespace Configurator.Installers
{
    public interface IAppConfigurator
    {
        void Configure(IApp app);
    }

    public class AppConfigurator : IAppConfigurator
    {
        private readonly IRegistryRepository registryRepository;

        public AppConfigurator(IRegistryRepository registryRepository)
        {
            this.registryRepository = registryRepository;
        }

        public void Configure(IApp app)
        {
            if (app.Configuration == null)
                return;

            app.Configuration.RegistrySettings.ForEach(setting =>
                registryRepository.SetValue(setting.KeyName, setting.ValueName, setting.ValueData));
        }
    }
}
