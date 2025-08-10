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
    public class XrayAIOptionServiceTest
    {
        private readonly Mock<IXrayAIOptionRepository> _repo = new();

        [Fact]
        public async Task Create_WhenAlreadyOne_ReturnsValidFailure()
        {
            _repo.Setup(r => r.IsOneAsync()).ReturnsAsync(true);
            var sut = new XrayAIOptionService(_repo.Object);

            var req = new AIOptionCreateRequest(
                OnnxModel: "model.onnx",
                ModelType: 0,
                Cuda: false,
                PrimeGpu: false,
                GpuId: 0,
                IsChecked: false,
                Confidence: 0.5,
                IoU: 0.5,
                Categories: []);

            var resp = await sut.Create(req);

            Assert.False(resp.Success);
            Assert.Equal(BaseRole.Valid, resp.Code);

            _repo.Verify(r => r.AddAsync(It.IsAny<AIOptionEntity>()), Times.Never);
        }

        [Fact]
        public async Task Create_Success_AddsOptionWithCategories()
        {
            _repo.Setup(r => r.IsOneAsync()).ReturnsAsync(false);
            _repo.Setup(r => r.AddAsync(It.IsAny<AIOptionEntity>())).Returns(Task.CompletedTask);
            var sut = new XrayAIOptionService(_repo.Object);

            var req = new AIOptionCreateRequest(
                OnnxModel: "model.onnx",
                ModelType: 1,
                Cuda: true,
                PrimeGpu: true,
                GpuId: 0,
                IsChecked: true,
                Confidence: 0.7,
                IoU: 0.6,
                Categories:
                [
                    new(){ },
                    new(){ },
                ]);

            var resp = await sut.Create(req);

            Assert.True(resp.Success);
            Assert.Equal(BaseRole.Success, resp.Code);
            Assert.NotNull(resp.Data);
            Assert.Equal(2, resp.Data!.Categories.Count);

            _repo.Verify(r => r.AddAsync(It.Is<AIOptionEntity>(
                e => e.OnnxModel == "model.onnx" && 
                e.Categories.Count == 2)), Times.Once);
        }

        [Fact]
        public async Task Save_WhenNoExisting_ReturnsValidFailure()
        {
            _repo.Setup(r => r.GetByIdSingleAsync()).ReturnsAsync((AIOptionEntity)null!);

            var sut = new XrayAIOptionService(_repo.Object);
            var req = new AIOptionSaveRequest([]);
            var resp = await sut.Save(req);

            Assert.False(resp.Success);
            Assert.Equal(BaseRole.Valid, resp.Code);
        }

        [Fact]
        public async Task Save_UpdatesCategories_AndReturnsOption()
        {
            var exist = new AIOptionEntity { Id = 1 };

            _repo.Setup(r => r.GetByIdSingleAsync()).ReturnsAsync(exist);
            _repo.Setup(r => r.UpdateCategoriesAsync(1, It.IsAny<ICollection<CategoryEntity>>())).ReturnsAsync(new AIOptionEntity { Id = 1, Categories = [new() { Id = 1, Name = "A" }] });

            var sut = new XrayAIOptionService(_repo.Object);
            var req = new AIOptionSaveRequest([ new() { 
                Id= 0, Name="A", Color="#f00", IsUsing=true, Confidence=0.5 }]);

            var resp = await sut.Save(req);

            Assert.True(resp.Success);
            Assert.NotNull(resp.Data);
            Assert.Single(resp.Data!.Categories);
        }

        [Fact]
        public async Task ExistOneRecord_ReturnsSuccessWhenExactlyOne()
        {
            _repo.Setup(r => r.IsOneAsync()).ReturnsAsync(true);

            var sut = new XrayAIOptionService(_repo.Object);
            var resp = await sut.ExistOneRecord();

            Assert.True(resp.Success);
            Assert.True(resp.Data);
        }

        [Fact]
        public async Task Load_WhenMissing_ReturnsValidFailure()
        {
            _repo.Setup(r => r.GetByIdSingleAsync()).ReturnsAsync((AIOptionEntity)null!);

            var sut = new XrayAIOptionService(_repo.Object);
            var resp = await sut.Load();

            Assert.False(resp.Success);
            Assert.Equal(BaseRole.Valid, resp.Code);
        }

        [Fact]
        public async Task Delete_AlwaysCallsDeleteAll_ReturnsTrue()
        {
            _repo.Setup(r => r.DeleteAllAsync()).Returns(Task.CompletedTask);

            var sut = new XrayAIOptionService(_repo.Object);
            var resp = await sut.Delete();

            Assert.True(resp.Success);
            Assert.True(resp.Data);

            _repo.Verify(r => r.DeleteAllAsync(), Times.Once);
        }
    }

}
