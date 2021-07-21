using NUnit.Framework;

namespace Emmersion.Http.UnitTests
{
    public class WhenDeserializingAJsonResponse
    {
        private HttpResponse response;
        private JsonTest deserialized;
        
        [SetUp]
        public void SetUp()
        {
            var json = "{\"stringProperty\":\"hello world\",\"IntegerProperty\":123}";
            response = new HttpResponse(200, new HttpHeaders(), json);
            deserialized = response.DeserializeJsonBody<JsonTest>();
        }

        [Test]
        public void ShouldReturnAnObjectWithTheCorrectProperties()
        {
            Assert.That(deserialized.StringProperty, Is.EqualTo("hello world"));
            Assert.That(deserialized.IntegerProperty, Is.EqualTo(123));
        }
    }
}