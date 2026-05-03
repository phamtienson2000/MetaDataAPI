using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MetadataApi.Dtos
{
    /// <summary>
    /// Represents the request body for creating a new metadata asset.
    /// Required fields are validated automatically by the ASP.NET Core model binding pipeline.
    /// </summary>
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
