using System.Collections.Generic;

namespace MetadataApi.Models
{
    /// <summary>
    /// Represents the internal domain model for a metadata asset.
    /// This model is used within the service layer and is never exposed directly via the API.
    /// </summary>
    public class MetadataAsset
    {
        public string Uid { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Owner { get; set; }
        public string SourceSystem { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
    }
}
