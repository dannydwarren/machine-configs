using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Emmersion.Http.IntegrationTests
{
    [TestFixture]
    public class HttpClientTests
    {
        [SetUp]
        public void SetUp()
        {
            client = new HttpClient();
        }

        [TearDown]
        public void TearDown()
        {
            client.Dispose();
        }

        private HttpClient client;

        private static HttpBinPostResponse GetHttpBinResponse(HttpResponse response)
        {
            return JsonConvert.DeserializeObject<HttpBinPostResponse>(response.Body);
        }

        [Test]
        public void ResponseIncludesContentHeaders()
        {
            var request = new HttpRequest {Url = "http://httpbin.org/post", Method = HttpMethod.POST, Body = ""};
            request.Headers.Add("Accept", "application/xml");
            request.Headers.Add("Content-Type", "application/json");

            var response = client.Execute(request);

            Assert.That(response.Headers.Exists("Content-Type"));
        }

        [Test]
        public void WhenAsyncRequestTakesLongerThanSpecifiedDefaultTimeout()
        {
            var request = new HttpRequest {Url = "http://httpbin.org/delay/3"};
            client = new HttpClient(new HttpClientOptions {DefaultTimeoutMilliseconds = 500});

            var task = client.ExecuteAsync(request);

            try
            {
                task.Wait(millisecondsTimeout: 10000);
                Assert.Fail("An exception should have been thrown");
            }
            catch (AggregateException ex)
            {
                Assert.That(ex.InnerExceptions.Any(e => e is HttpTimeoutException), Is.True, "The exception inside the AggregateException should be an HttpTimeoutException");
            }
        }

        [Test]
        public void WhenMakingMultipleAsyncRequestsWithSpecifiedTimeout()
        {
            var request = new HttpRequest {Url = "http://httpbin.org/delay/3"};

            var task1 = new HttpClient(new HttpClientOptions {DefaultTimeoutMilliseconds = 5000}).ExecuteAsync(request);
            var task2 = new HttpClient(new HttpClientOptions {DefaultTimeoutMilliseconds = 500}).ExecuteAsync(request);
            var task3 = new HttpClient(new HttpClientOptions {DefaultTimeoutMilliseconds = 10000}).ExecuteAsync(request);
            var task4 = new HttpClient(new HttpClientOptions {DefaultTimeoutMilliseconds = 1000}).ExecuteAsync(request);

            try
            {
                Task.WaitAll(task1, task2, task3, task4);
                Assert.Fail("Some requests should have failed");
            }
            catch (AggregateException ex)
            {
                Assert.That(ex.InnerExceptions.Count, Is.EqualTo(expected: 2));
                foreach (var innerException in ex.InnerExceptions)
                {
                    Assert.That(innerException, Is.InstanceOf<HttpTimeoutException>());
                }
            }

            Assert.That(task1.IsFaulted, Is.False);
            Assert.That(task2.IsFaulted, Is.True);
            Assert.That(task3.IsFaulted, Is.False);
            Assert.That(task4.IsFaulted, Is.True);
        }

        [Test]
        public void WhenMakingMultipleRequestsQuicklyWithSpecifiedTimeout()
        {
            var request = new HttpRequest {Url = "http://httpbin.org/get"};

            for (var i = 0; i < 20; i++)
            {
                var response = client.Execute(request, timeoutMilliseconds: 1000);
                Assert.That(response.StatusCode, Is.EqualTo(expected: 200));
            }
        }

        [Test]
        public void WhenPerformingSimpleDelete()
        {
            var request = new HttpRequest {Url = "http://httpbin.org/delete", Method = HttpMethod.DELETE};

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(expected: 200));
        }

        [Test]
        public void WhenPerformingSimpleGet()
        {
            var request = new HttpRequest {Url = "http://httpbin.org/get?foo=bar"};

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(expected: 200));
            Assert.That(response.Body, Is.Not.Empty);
            Assert.That(response.Headers.GetAllHeaderNames().Count, Is.GreaterThan(expected: 0));

            dynamic responseBody = JsonConvert.DeserializeObject(response.Body);
            Assert.That((string) responseBody["args"]["foo"], Is.EqualTo("bar"));
        }

        [Test]
        public void WhenPerformingGetAsAStream()
        {
            var request = new HttpRequest {Url = "http://httpbin.org/get?foo=bar"};

            var response = client.ExecuteAsStream(request);

            Assert.That(response.StatusCode, Is.EqualTo(expected: 200));
            Assert.That(response.Stream, Has.Length.GreaterThan(0));
            Assert.That(response.Headers.GetAllHeaderNames().Count, Is.GreaterThan(expected: 0));

            using var streamReader = new StreamReader(response.Stream);
            var json = streamReader.ReadToEnd();
            dynamic responseBody = JsonConvert.DeserializeObject(json);
            Assert.That((string) responseBody["args"]["foo"], Is.EqualTo("bar"));
        }

        [Test]
        public void WhenPerformingSimplePostingWithJson()
        {
            var request = new HttpRequest {Url = "http://httpbin.org/post", Method = HttpMethod.POST, Body = "{\"username\":\"standard-user\", \"password\":\"testing1\"}"};

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(expected: 200));
            var responseData = GetHttpBinResponse(response);
            Assert.That(responseData.Data, Is.EqualTo(request.Body));
        }

        [Test]
        public void WhenPostingWithJsonAsAStream()
        {
            var body = "{\"username\":\"standard-user\", \"password\":\"testing1\"}";
            var asStream = new MemoryStream(Encoding.UTF8.GetBytes(body));
            var request = new StreamHttpRequest{Url = "http://httpbin.org/post", Method = HttpMethod.POST, Body = asStream };

            request.Headers.Add("Content-Type", "application/json");

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(expected: 200));
            var responseData = GetHttpBinResponse(response);
            Assert.That(responseData.Data, Is.EqualTo(body));
        }

        [Test]
        public void WhenPerformingSimplePut()
        {
            var request = new HttpRequest {Url = "http://httpbin.org/put", Method = HttpMethod.PUT, Body = "some data"};

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(expected: 200));
            var responseData = GetHttpBinResponse(response);
            Assert.That(responseData.Data, Is.EqualTo(request.Body));
        }

        [Test]
        public void WhenPermanentRedirectShouldNotBeFollowed()
        {
            var request = new HttpRequest {Url = "https://jigsaw.w3.org/HTTP/300/301.html"};
            client = new HttpClient(new HttpClientOptions {AllowAutoRedirect = false});

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(expected: 301));
            Assert.That(response.Body, Is.Not.Empty);
            Assert.That(response.Headers.GetValue("location"), Is.EqualTo("https://jigsaw.w3.org/HTTP/300/Overview.html").IgnoreCase);
        }

        [Test]
        public void WhenRequestTakesLongerThanSpecifiedDefaultTimeout()
        {
            var request = new HttpRequest {Url = "http://httpbin.org/delay/3"};
            client = new HttpClient(new HttpClientOptions {DefaultTimeoutMilliseconds = 500});

            Assert.Throws<HttpTimeoutException>(() => client.Execute(request));
        }

        [Test]
        public void WhenRequestTakesLongerThanSpecifiedTimeout()
        {
            var request = new HttpRequest {Url = "http://httpbin.org/delay/3"};

            Assert.Throws<HttpTimeoutException>(() => client.Execute(request, timeoutMilliseconds: 500));
        }

        [Test]
        public void WhenSendingCookies()
        {
            var request = new HttpRequest {Url = "http://httpbin.org/headers"};
            request.Headers.Add("cookie", "monster");

            var response = client.Execute(request);
            var headers = (JObject) JObject.Parse(response.Body).GetValue("headers");

            Assert.That(headers["Cookie"].Value<string>(), Is.EqualTo("monster"));
        }

        [Test]
        public void WhenSendingHeaders()
        {
            var request = new HttpRequest {Url = "http://httpbin.org/headers"};
            request.Headers.Add("Authorization", "token abc123");
            request.Headers.Add("x-bentern", "kobe");

            var response = client.Execute(request);
            var headers = (JObject) JObject.Parse(response.Body).GetValue("headers");
            Assert.That(headers["Authorization"].Value<string>(), Is.EqualTo("token abc123"));
            Assert.That(headers["X-Bentern"].Value<string>(), Is.EqualTo("kobe"));
        }

        [Test]
        public void WhenSendingSpecialHeaders()
        {
            var request = new HttpRequest {Url = "http://httpbin.org/post", Method = HttpMethod.POST, Body = ""};
            request.Headers.Add("Accept", "application/xml");
            request.Headers.Add("Content-Type", "application/json");

            var response = client.Execute(request);

            Assert.That(response.Body, Does.Match("\"Accept\":\\s*\"application/xml\""));
            Assert.That(response.Body, Does.Match("\"Content-Type\":\\s*\"application/json\""));
        }

        [Test]
        public void WhenSendingUserAgentHeader()
        {
            var request = new HttpRequest {Url = "http://httpbin.org/user-agent", Method = HttpMethod.GET};
            request.Headers.Add("user-agent", "custom user agent");

            var response = client.Execute(request);

            Assert.That(response.Body, Does.Contain("custom user agent"));
        }

        [Test]
        public void WhenTemporaryRedirectShouldBeFollowed()
        {
            var request = new HttpRequest {Url = "https://httpbingo.org/redirect-to?url=https://github.com"};
            client = new HttpClient(new HttpClientOptions {AllowAutoRedirect = true});

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(expected: 200));
            Assert.That(response.Body, Is.Not.Empty);
            Assert.That(response.Headers.GetAllHeaderNames().Count, Is.GreaterThan(expected: 0));
            Assert.That(response.Headers.GetValue("server"), Is.EqualTo("github.com").IgnoreCase);
            Assert.That(response.Headers.GetValue("location"), Is.Empty);
        }

        [Test]
        public void WhenTemporaryRedirectShouldNotBeFollowed()
        {
            var request = new HttpRequest {Url = "https://httpbingo.org/redirect-to?url=https://github.com"};
            client = new HttpClient(new HttpClientOptions {AllowAutoRedirect = false});

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(expected: 302));
            Assert.That(response.Body, Is.Empty);
            Assert.That(response.Headers.GetAllHeaderNames().Count, Is.GreaterThan(expected: 0));
            Assert.That(response.Headers.GetValue("server"), Is.Not.EqualTo("github.com").IgnoreCase);
            Assert.That(response.Headers.GetValue("location"), Is.EqualTo("https://github.com/").IgnoreCase);
        }
    }

    public class HttpBinPostResponse
    {
        public string Data { get; set; }
    }
}
