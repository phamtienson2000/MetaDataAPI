using MetadataApi.Models;
using System.Collections.Generic;

namespace MetadataApi.Services
{
    /// <summary>
    /// Defines the contract for metadata asset operations.
    /// and future replacement of the storage implementation.
    /// </summary>
    public interface IMetadataAssetService
    {
        MetadataAsset? GetByUid(string uid);
        IEnumerable<MetadataAsset> GetAll(string? type = null);
        (bool Success, string? Error, MetadataAsset? Asset) Create(MetadataAsset asset);
    }
}
