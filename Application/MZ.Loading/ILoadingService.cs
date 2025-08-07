using MZ.Model;

namespace MZ.Loading
{
    public interface ILoadingService
    {
        LoadingModel this[string regionName] { get; }
    }
}
