using Moq;
using MZ.Domain.Entities;
using MZ.DTO.Enums;
using MZ.DTO;
using MZ.Domain.Enums;
using MZ.Infrastructure.Interfaces;
using MZ.Infrastructure.Services;
using Xunit;
using Assert = Xunit.Assert;

namespace MZ.Infrastructure.Test
{
    public class XrayVisionZeffectControlServiceTest
    {

        private readonly Mock<IUserRepository> _users = new();
        private readonly Mock<IUserSession> _session = new();
        private readonly Mock<IXrayVisionZeffectControlRepository> _repo = new();

        public UserEntity MakeUser(int id = 1, string username = "sample")
        {
            return new UserEntity() { Id = id, Username = username, Role = UserRole.User };
        }

        private XrayVisionZeffectControlService CreateSut()
        {
            return new(_users.Object, _session.Object, _repo.Object);
        }

        [Fact]
        public async Task Load_ByUsername_WhenNone_ReturnsValidFailure()
        {
            var user = MakeUser(1, "sample");
            _users.Setup(r => r.GetByUsername("sample")).Returns(user);
            _repo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync([]);

            var sut = CreateSut();
            var resp = await sut.Load(new ZeffectControlLoadRequest("sample"));

            Assert.False(resp.Success);
            Assert.Equal(BaseRole.Valid, resp.Code);
        }

        [Fact]
        public async Task Save_ReplacesAndReturnsAll()
        {
            _session.SetupGet(s => s.CurrentUser).Returns("sample");
            var user = MakeUser(1, "sample");

            _users.Setup(r => r.GetByUsername("sample")).Returns(user);

            var existing = new List<ZeffectControlEntity>
            {
                new(){ Id=10, UserId=1, Check=true,  Content="A"},
                new(){ Id=11, UserId=1, Check=false, Content="B"},
            };

            _repo.SetupSequence(r => r.GetByUserIdAsync(1))
                 .ReturnsAsync(existing) 
                 .ReturnsAsync([
                    new(){ Id=20, UserId=1 }, new(){ Id=21, UserId=1 }
                 ]);

            _repo.Setup(r => r.DeleteAsync(It.IsAny<ZeffectControlEntity>())).Returns(Task.CompletedTask);
            _repo.Setup(r => r.UpdateAsync(It.IsAny<ZeffectControlEntity>())).Returns(Task.CompletedTask);
            _repo.Setup(r => r.AddAsync(It.IsAny<ZeffectControlEntity>())).Returns(Task.CompletedTask);

            var sut = CreateSut();
            var req = new ZeffectControlSaveRequest(
            [
                new(){ Id=0,  Check=true, Content="N1", Min=0, Max=10, Color="#f00"},
                new(){ Id=11, Check=true, Content="B*", Min=1, Max=9,  Color="#0f0"},
            ]);

            var resp = await sut.Save(req);

            Assert.True(resp.Success);
            Assert.Equal(2, resp.Data!.Count);

            _repo.Verify(r => r.DeleteAsync(It.Is<ZeffectControlEntity>(z => z.Id == 10)), Times.Once);
            _repo.Verify(r => r.UpdateAsync(It.Is<ZeffectControlEntity>(z => z.Id == 11 && z.Check && z.Content == "B*")), Times.Once);
            _repo.Verify(r => r.AddAsync(It.Is<ZeffectControlEntity>(z => z.Id == 0 && z.UserId == 1)), Times.Once);
        }
    }
}
