using Microsoft.Identity.Client;
using RichardSzalay.MockHttp;
using System;
using System.Linq;
using Xunit;

namespace paule96.Onedrive.AlbumDownloader.Tests
{
    public class GraphSessionServiceTest
    {
        [Fact]
        public async void GetHttpContentWithAnAuthResult()
        {
            // Assert            
            var content = "{" +
                        "\"@odata.context\": \"https://test.local/$metadata\"," +
                        "\"value\": [{" +
                            "\"aProperty\": 5" +
                        "}]" +
                    "}";
            var url = "https://test.local/";
            var accessToken = "youHaveAccess";

            var mockHttp = new MockHttpMessageHandler();
            // returns the right response if the request is right
            var mockedResponse = mockHttp.When(url)
                    .Respond("application/json", content)
                    .WithHeaders("Authorization", "Bearer " + accessToken);

            IGraphSessionService graphService = new GraphSessionService(Guid.NewGuid().ToString(), mockHttp.ToHttpClient());
            var authResult = new AuthenticationResult(accessToken, true, string.Empty, DateTimeOffset.Now, DateTimeOffset.Now, string.Empty, null, string.Empty, null);

            // Act
            var responseContent = await graphService.GetHttpContentWithToken(url, authResult);

            // Arrange
            Assert.Equal(1, mockHttp.GetMatchCount(mockedResponse));
            Assert.Equal(content, responseContent);
        }

        [Fact]
        public async void GetSingleItemFromUri()
        {
            // Assert            
            var content = "{" +
                            "\"@odata.context\": \"https://test.local/$metadata\"," +
                            "\"aProperty\": 5" +
                        "}";
            var url = "https://test.local/";
            var accessToken = "youHaveAccess";

            var mockHttp = new MockHttpMessageHandler();
            // returns the right response if the request is right
            var mockedResponse = mockHttp.When(url)
                    .Respond("application/json", content)
                    .WithHeaders("Authorization", "Bearer " + accessToken);

            IGraphSessionService graphService = new GraphSessionService(Guid.NewGuid().ToString(), mockHttp.ToHttpClient());
            var authResult = new AuthenticationResult(accessToken, true, string.Empty, DateTimeOffset.Now, DateTimeOffset.Now, string.Empty, null, string.Empty, null);

            // Act
            var responseContent = await graphService.GetSingleContent<SampleData>(url, authResult);

            // Arrange
            Assert.Equal(1, mockHttp.GetMatchCount(mockedResponse));
            Assert.Equal(5, responseContent.aProperty);
        }

        [Fact]
        public async void GetMultipleItemsFromUri()
        {
            // Assert            
            var content = "{" +
                            "\"@odata.context\": \"https://test.local/$metadata\"," +
                            "\"value\": [{" +
                                "\"aProperty\": 5" +
                            "}]" +
                        "}";
            var url = "https://test.local/";
            var accessToken = "youHaveAccess";

            var mockHttp = new MockHttpMessageHandler();
            // returns the right response if the request is right
            var mockedResponse = mockHttp.When(url)
                    .Respond("application/json", content)
                    .WithHeaders("Authorization", "Bearer " + accessToken);

            IGraphSessionService graphService = new GraphSessionService(Guid.NewGuid().ToString(), mockHttp.ToHttpClient());
            var authResult = new AuthenticationResult(accessToken, true, string.Empty, DateTimeOffset.Now, DateTimeOffset.Now, string.Empty, null, string.Empty, null);

            // Act
            var responseContent = await graphService.GetListContent<SampleData>(url, authResult);

            // Arrange
            Assert.Equal(1, mockHttp.GetMatchCount(mockedResponse));
            Assert.Single(responseContent);
            Assert.Equal(5, responseContent.FirstOrDefault().aProperty);
        }

        [Fact]
        public async void GetMultipleItemsFromUriWithManyPages()
        {
            // Assert 
            var nextUrl = "https://test.local/2";
            var firstContent = "{" +
                            "\"@odata.context\": \"https://test.local/$metadata\"," +
                            "\"@odata.nextLink\": \"" + nextUrl + "\"," +
                            "\"value\": [{" +
                                "\"aProperty\": 5" +
                            "}]" +
                        "}";
            var secoundContent = "{" +
                            "\"@odata.context\": \"https://test.local/$metadata\"," +
                            "\"value\": [{" +
                                "\"aProperty\": 7" +
                            "}]" +
                        "}";
            var url = "https://test.local/";
            var accessToken = "youHaveAccess";

            var mockHttp = new MockHttpMessageHandler();
            // returns the right response if the request is right
            var mockedFirstResponse = mockHttp.When(url)
                    .Respond("application/json", firstContent)
                    .WithHeaders("Authorization", "Bearer " + accessToken);
            var mockedSecoundResponse = mockHttp.When(nextUrl)
                    .Respond("application/json", secoundContent)
                    .WithHeaders("Authorization", "Bearer " + accessToken);

            IGraphSessionService graphService = new GraphSessionService(Guid.NewGuid().ToString(), mockHttp.ToHttpClient());
            var authResult = new AuthenticationResult(accessToken, true, string.Empty, DateTimeOffset.Now, DateTimeOffset.Now, string.Empty, null, string.Empty, null);

            // Act
            var responseContent = await graphService.GetListContent<SampleData>(url, authResult);

            // Arrange
            Assert.Equal(1, mockHttp.GetMatchCount(mockedFirstResponse));
            Assert.Equal(2, responseContent.Count());
            Assert.Equal(5, responseContent.FirstOrDefault().aProperty);
            Assert.Single(responseContent.Where(d => d.aProperty == 7));
        }

        [Fact]
        public async void GetMultipleItemsThrowsErrorIfTheResponseIsNotAList()
        {
            // Assert            
            var content = "{" +
                            "\"@odata.context\": \"https://test.local/$metadata\"," +
                            "\"aProperty\": 5" +
                        "}";
            var url = "https://test.local/";
            var accessToken = "youHaveAccess";

            var mockHttp = new MockHttpMessageHandler();
            // returns the right response if the request is right
            var mockedResponse = mockHttp.When(url)
                    .Respond("application/json", content)
                    .WithHeaders("Authorization", "Bearer " + accessToken);

            IGraphSessionService graphService = new GraphSessionService(Guid.NewGuid().ToString(), mockHttp.ToHttpClient());
            var authResult = new AuthenticationResult(accessToken, true, string.Empty, DateTimeOffset.Now, DateTimeOffset.Now, string.Empty, null, string.Empty, null);

            // Act & Arrange                
            await Assert.ThrowsAsync<Exception>(async () => await graphService.GetListContent<SampleData>(url, authResult));            
        }

        private class SampleData
        {
            public int aProperty { get; set; }
        }
    }
}
