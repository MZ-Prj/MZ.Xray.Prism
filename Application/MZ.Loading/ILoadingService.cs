
using MZ.Domain.Models;

namespace MZ.Loading
{
    public interface ILoadingService
    {
        LoadingModel this[string regionName] { get; }
    }
}
