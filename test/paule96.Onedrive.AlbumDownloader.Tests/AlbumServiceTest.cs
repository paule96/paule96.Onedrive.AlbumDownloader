using Microsoft.Identity.Client;
using Moq;
using paule96.Onedrive.AlbumDownloader.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace paule96.Onedrive.AlbumDownloader.Tests
{
    public class AlbumServiceTest
    {
        public Mock<IGraphSessionService> graphSessionServiceMock { get; set; }
        public string userId = Guid.NewGuid().ToString();
        public AlbumServiceTest()
        {
            graphSessionServiceMock = new Mock<IGraphSessionService>();
            graphSessionServiceMock
                .Setup(d => d.CurrentUser)
                .Returns(new Models.User()
                {
                    id = userId
                });
        }

        [Fact]
        public async void GetAListOfAlbums()
        {
            // Arrange
            graphSessionServiceMock
                .Setup(d => d.GetListContent<Album>(It.IsAny<string>()))
                .ReturnsAsync(new List<Album>());
            var expectedUrl = $"https://graph.microsoft.com/v1.0/me/drive/items/{userId}!0:/SkyDriveCache/Albums:/children";
            // Act
            var result = await (new AlbumService(graphSessionServiceMock.Object)).GetAlbums();

            // Assert
            graphSessionServiceMock
                .Verify(d => d.GetListContent<Album>(
                    expectedUrl),
                    $"The url is wrong to crawl albums. The expected url was: {expectedUrl}");
            Assert.Empty(result);
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetAListOfAlbumItems()
        {
            // Arrange
            graphSessionServiceMock
                .Setup(d => d.GetListContent<File>(It.IsAny<string>()))
                .ReturnsAsync(new List<File>());
            var albumId = Guid.NewGuid().ToString();
            var expectedUrl = $"https://graph.microsoft.com/v1.0/drives/{userId}/items/{albumId}/children";
            // Act
            var result = await (new AlbumService(graphSessionServiceMock.Object)).GetAlbumItems(albumId);

            // Assert
            graphSessionServiceMock
                .Verify(d => d.GetListContent<File>(
                    expectedUrl),
                    $"The url is wrong to crawl albums. The expected url was: {expectedUrl}");
            Assert.Empty(result);
            Assert.NotNull(result);
        }
    }
}
