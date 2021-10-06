using Configurator.Utilities;
using Shouldly;
using Xunit;

namespace Configurator.UnitTests.Utilities
{
    public class TokenizerTests : UnitTestBase<Tokenizer>
    {
        [Fact]
        public void When_detokenizing_a_single_environment_token()
        {
            var tokenizedString = "{{env:ProgramFiles}}\\some-program-folder";

            var detokenizedString = Because(() => ClassUnderTest.Detokenize(tokenizedString));

            It("Replaces Environment Tokens", () => detokenizedString.ShouldBe("C:\\Program Files\\some-program-folder"));
        }

        [Fact]
        public void When_detokenizing_multiple_environment_tokens()
        {
            var tokenizedString = "{{env:ProgramFiles}}\\some-program-folder\\{{env:ProgramData}}";

            var detokenizedString = Because(() => ClassUnderTest.Detokenize(tokenizedString));

            It("Replaces Environment Tokens", () => detokenizedString.ShouldBe("C:\\Program Files\\some-program-folder\\C:\\ProgramData"));
        }
    }
}
