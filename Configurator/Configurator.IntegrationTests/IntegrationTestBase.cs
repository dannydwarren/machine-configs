using SpecBecause;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Configurator.IntegrationTests.Configuration;

namespace Configurator.IntegrationTests
{
    public class IntegrationTestBase<TClassUnderTest> : IEngine where TClassUnderTest : class
    {
        private ServiceProvider serviceProvider;

        private IEngine Engine { get; }

        public IntegrationTestBase(IEngine? engine = null)
        {
            Engine = engine ?? new Engine();

            var serviceCollection = new ServiceCollection();
            RegisterRequiredServices(serviceCollection);
            RegisterTestSpecificServices(serviceCollection);
            serviceProvider = serviceCollection.BuildServiceProvider();
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
            serviceProvider.Dispose();
        }

        private TClassUnderTest? classUnderTest;
        protected TClassUnderTest ClassUnderTest => classUnderTest ??= serviceProvider.GetService<TClassUnderTest>()!;

        protected virtual void RegisterTestSpecificServices(ServiceCollection services) { }

        protected U GetInstance<U>()
        {
            return serviceProvider.GetService<U>()!;
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
