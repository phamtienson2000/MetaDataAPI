using System.Collections.Generic;

namespace MetadataApi.Dtos
{
    public class MetadataAssetResponse
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
