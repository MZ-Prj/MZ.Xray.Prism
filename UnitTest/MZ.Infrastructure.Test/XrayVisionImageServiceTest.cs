using Moq;
using MZ.Domain.Entities;
using MZ.DTO.Enums;
using MZ.DTO;
using MZ.Infrastructure.Interfaces;
using MZ.Infrastructure.Services;
using Xunit;
using Assert = Xunit.Assert;

namespace MZ.Infrastructure.Test
{
    public class XrayVisionImageServiceTest
    {
        private readonly Mock<IUserRepository> _users = new();
        private readonly Mock<IXrayVisionImageRepository> _repo = new();

        private XrayVisionImageService CreateSut()
        {
            return new(_users.Object, _repo.Object);
        }

        [Fact]
        public async Task Load_PageQuery_WhenEmpty_ReturnsValidFailure()
        {
            _repo.Setup(r => r.GetByDateTimeBetweenStartEndAndPageSize(
                    It.IsAny<DateTime>(), It.IsAny<DateTime>(), 1, 10))
                 .ReturnsAsync([]);

            var sut = CreateSut();
            var req = new ImageLoadRequest(DateTime.Now.AddDays(-1), DateTime.Now, 1, 10);
            var resp = await sut.Load(req);

            Assert.False(resp.Success);
            Assert.Equal(BaseRole.Valid, resp.Code);
        }

        [Fact]
        public async Task Load_PageQuery_ReturnsImages()
        {
            var list = new List<ImageEntity> { 
                new() { Id = 1 }, 
                new() { Id = 2 } 
            };
            _repo.Setup(r => r.GetByDateTimeBetweenStartEndAndPageSize(
                    It.IsAny<DateTime>(), It.IsAny<DateTime>(), 1, 10))
                 .ReturnsAsync(list);

            var sut = CreateSut();
            var req = new ImageLoadRequest(DateTime.Now.AddDays(-1), DateTime.Now, 1, 10);
            var resp = await sut.Load(req);

            Assert.True(resp.Success);
            Assert.Equal(2, resp.Data!.Count);
        }

        [Fact]
        public async Task Load_ReportQuery_WhenEmpty_ReturnsValidFailure()
        {
            _repo.Setup(r => r.GetByDateTimeBetweenStartEnd(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                 .ReturnsAsync([]);

            var sut = CreateSut();
            var req = new ReportImageLoadRequest(DateTime.Now.AddDays(-30), DateTime.Now);
            var resp = await sut.Load(req);

            Assert.False(resp.Success);
            Assert.Equal(BaseRole.Valid, resp.Code);
        }

        [Fact]
        public async Task Save_BuildsEntityAndAdds()
        {
            _repo.Setup(r => r.AddAsync(It.IsAny<ImageEntity>())).Returns(Task.CompletedTask);

            var sut = CreateSut();
            var req = new ImageSaveRequest(
                Path: "C:\\data",
                Filename: "a.png",
                Width: 100,
                Height: 50,
                ObjectDetections: [
                    new(){ X=1, Y=2, Width=3, Height=4, Confidence=0.9, Name="A"}
                ]);

            var resp = await sut.Save(req);

            Assert.True(resp.Success);

            _repo.Verify(r => r.AddAsync(It.Is<ImageEntity>(e =>
                e.Path.EndsWith(Path.Combine("C:\\data", "Image")) &&
                e.Filename == "a.png" &&
                e.Width == 100 &&
                e.Height == 50 &&
                e.ObjectDetections.Count == 1
            )), Times.Once);
        }
    }
}
