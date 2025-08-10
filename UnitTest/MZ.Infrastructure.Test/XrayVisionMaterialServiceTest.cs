using Moq;
using MZ.Domain.Entities;
using MZ.DTO.Enums;
using MZ.DTO;
using MZ.Infrastructure.Interfaces;
using MZ.Infrastructure.Services;
using MZ.Domain.Enums;
using Xunit;
using Assert = Xunit.Assert;

namespace MZ.Infrastructure.Test
{
    public class XrayVisionMaterialServiceTest
    {

        private readonly Mock<IUserRepository> _users = new();
        private readonly Mock<IUserSession> _session = new();
        private readonly Mock<IXrayVisionMaterialRepository> _repo = new();
        private readonly Mock<IXrayVisionMaterialControlRepository> _ctrlRepo = new();

        public UserEntity MakeUser(int id = 1, string username = "sample")
        {
            return new UserEntity() { Id = id, Username = username, Role = UserRole.User };
        }

        private XrayVisionMaterialService CreateSut()
        {
            return new(_users.Object, _session.Object, _repo.Object, _ctrlRepo.Object);
        }

        [Fact]
        public async Task Load_ByUsername_WhenMissing_ReturnsValidFailure()
        {
            var user = MakeUser(1, "sample");

            _users.Setup(r => r.GetByUsername("sample")).Returns(user);
            _repo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync((MaterialEntity)null!);

            var sut = CreateSut();
            var resp = await sut.Load(new MaterialLoadRequest("sample"));

            Assert.False(resp.Success);
            Assert.Equal(BaseRole.Valid, resp.Code);
        }

        [Fact]
        public async Task Save_NewMaterial_AddsWithControls()
        {
            _session.SetupGet(s => s.CurrentUser).Returns("sample");
            var user = MakeUser(1, "sample");

            _users.Setup(r => r.GetByUsername("sample")).Returns(user);
            _repo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync((MaterialEntity)null!);
            _repo.Setup(r => r.AddAsync(It.IsAny<MaterialEntity>())).Returns(Task.CompletedTask);

            var reqCtrls = new List<MaterialControlEntity>
            {
                new(){ Id=0, Y=1, XMin=2, XMax=3, Color="#f00" },
                new(){ Id=0, Y=2, XMin=5, XMax=8, Color="#0f0" },
            };
            var sut = CreateSut();
            var req = new MaterialSaveRequest(1, 2, 3, 4, 5, reqCtrls);

            var resp = await sut.Save(req);

            Assert.True(resp.Success);

            _repo.Verify(r => r.AddAsync(It.Is<MaterialEntity>(m =>
                m.UserId == 1 && m.MaterialControls.Count == 2)), Times.Once);
        }

        [Fact]
        public async Task Save_Update_DeletesMissingAndAddsOrUpdatesExistingControls()
        {
            _session.SetupGet(s => s.CurrentUser).Returns("sample");
            var user = MakeUser(1, "sample");
            _users.Setup(r => r.GetByUsername("sample")).Returns(user);

            var existing = new MaterialEntity
            {
                Id = 10,
                UserId = 1,
                MaterialControls =
                [
                    new(){ Id=100, Y=0, XMin=0, XMax=0, Color="#f00" },
                    new(){ Id=101, Y=9, XMin=9, XMax=9, Color="#0f0" },
                ]
            };
            _repo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(existing);

            _ctrlRepo.Setup(r => r.DeleteAsync(It.IsAny<MaterialControlEntity>())).Returns(Task.CompletedTask);
            _ctrlRepo.Setup(r => r.AddAsync(It.IsAny<MaterialControlEntity>())).Returns(Task.CompletedTask);
            _ctrlRepo.Setup(r => r.UpdateAsync(It.IsAny<MaterialControlEntity>())).Returns(Task.CompletedTask);

            var reqCtrls = new List<MaterialControlEntity>
            {
                new(){ Id=101, Y=1, XMin=2, XMax=3, Color="#f00" },
                new(){ Id=0,   Y=5, XMin=6, XMax=7, Color="#0f0" },
            };

            var sut = CreateSut();
            var req = new MaterialSaveRequest(1, 2, 3, 4, 5, reqCtrls);

            var resp = await sut.Save(req);

            Assert.True(resp.Success);
            _ctrlRepo.Verify(r => r.DeleteAsync(It.Is<MaterialControlEntity>(c => c.Id == 100)), Times.Once);
            _ctrlRepo.Verify(r => r.UpdateAsync(It.Is<MaterialControlEntity>(c => c.Id == 101 && c.Y == 1)), Times.Once);
            _ctrlRepo.Verify(r => r.AddAsync(It.Is<MaterialControlEntity>(c => c.Id == 0 && c.Y == 5)), Times.Once);
        }
    }
}
