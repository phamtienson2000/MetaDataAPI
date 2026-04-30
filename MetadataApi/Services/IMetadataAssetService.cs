using MetadataApi.Models;
using System.Collections.Generic;

namespace MetadataApi.Services
{
    public interface IMetadataAssetService
    {
        MetadataAsset? GetByUid(string uid);
        IEnumerable<MetadataAsset> GetAll(string? type = null);
        (bool Success, string? Error, MetadataAsset? Asset) Create(MetadataAsset asset);
    }
}
