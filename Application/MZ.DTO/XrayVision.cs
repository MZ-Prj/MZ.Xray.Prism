
using System;

namespace MZ.DTO
{
#nullable enable

    #region Request
    public record ImageSaveRequest(
        string Path,
        string Filename,
        int Width,
        int Height
    );

    public record ImageLoadRequest(
        DateTime Start,
        DateTime End,
        int Page,
        int Size
    );
    #endregion

    #region Response
    #endregion

    #region DTO
    #endregion
}
