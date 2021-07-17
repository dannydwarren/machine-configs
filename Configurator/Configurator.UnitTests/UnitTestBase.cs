using SpecBecause;
using System;
using System.Linq;
using AutoMoqCore;
using Moq;
using System.Collections.Generic;

namespace Configurator.UnitTests
{
    public class UnitTestBase<TClassUnderTest>: IEngine
    {
        private IEngine Engine { get; }
        private AutoMoqer mocker;

        public UnitTestBase(IEngine? engine = null)
        {
            Engine = engine ?? new Engine();
            mocker = new AutoMoqer();
        }

        public void Because(Action act)
        {
            Engine.Because(act);
        }

        public TResult Because<TResult>(Func<TResult> act)
        {
            return Engine.Because(act);
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
            classUnderTest = default;
            Engine.Dispose();
        }

        private TClassUnderTest? classUnderTest;
        protected TClassUnderTest ClassUnderTest => classUnderTest ??= mocker.Create<TClassUnderTest>();

        protected Mock<TMock> GetMock<TMock>() where TMock : class
        {
            return mocker.GetMock<TMock>();
        }

        protected TAny IsAny<TAny>()
        {
            return Moq.It.IsAny<TAny>();
        }

        protected List<TAny> IsSequenceEqual<TAny>(IEnumerable<TAny> collection)
        {
            return Moq.It.Is<List<TAny>>(x => x.SequenceEqual(collection));
        }

        protected Guid NewGuid() => Guid.NewGuid();

        protected string RandomString() => Guid.NewGuid().ToString();
    }
}
