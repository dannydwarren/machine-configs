using SpecBecause;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Configurator.IntegrationTests.Configuration;
using Configurator.Utilities;
using Configurator.Windows;

namespace Configurator.IntegrationTests
{
    public class IntegrationTestBase<TClassUnderTest> : IEngine where TClassUnderTest : class
    {
        private ServiceProvider? serviceProvider;
        private IEngine Engine { get; }

        protected ServiceCollection Services { get; }

        public IntegrationTestBase(IEngine? engine = null)
        {
            Engine = engine ?? new Engine();

            Services = new ServiceCollection();
            RegisterRequiredServices(Services);
        }

        public void Because(Action act)
        {
            Engine.Because(act);
        }

        public async Task BecauseAsync(Func<Task> act)
        {
            await Engine.Because(act);
        }

        public TResult Because<TResult>(Func<TResult> act)
        {
            return Engine.Because(act);
        }

        public async Task<TResult> BecauseAsync<TResult>(Func<Task<TResult>> act)
        {
            return await Engine.Because(act);
        }

        public TException? BecauseThrows<TException>(Action act) where TException : Exception
        {
            return Engine.BecauseThrows<TException>(act);
        }

        public void It(string assertionMessage, Action assertion)
        {
            Engine.It(assertionMessage, assertion);
        }

        public void Dispose()
        {
            Engine.Dispose();
            serviceProvider?.Dispose();
        }

        private TClassUnderTest? classUnderTest;
        protected TClassUnderTest ClassUnderTest
        {
            get
            {
                if (classUnderTest == null)
                {
                    serviceProvider = Services.BuildServiceProvider();
                    RegistrySettingValueDataConverter.Tokenizer = serviceProvider.GetRequiredService<ITokenizer>();

                    classUnderTest = serviceProvider.GetService<TClassUnderTest>()!;
                }

                return classUnderTest;
            }
        }

        protected TService GetInstance<TService>()
        {
            return serviceProvider!.GetService<TService>()!;
        }

        private static void RegisterRequiredServices(ServiceCollection services)
        {
            DependencyInjectionConfig.ConfigureServices(services);

            services.AddTransient<TClassUnderTest, TClassUnderTest>();
        }

        protected Guid NewGuid() => Guid.NewGuid();

        protected string RandomString() => Guid.NewGuid().ToString();
    }
}
