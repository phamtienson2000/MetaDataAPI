using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MetadataApi.Dtos
{
    public class CreateMetadataAssetRequest
    {
        [Required]
        public string? Uid { get; set; }

        [Required]
        public string? Type { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Owner { get; set; }

        [Required]
        public string? SourceSystem { get; set; }

        public List<string>? Tags { get; set; }
    }
}
