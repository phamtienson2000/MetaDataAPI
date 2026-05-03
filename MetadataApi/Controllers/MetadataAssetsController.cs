using MetadataApi.Dtos;
using MetadataApi.Models;
using MetadataApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MetadataApi.Controllers
{
    /// <summary>
    /// Controller responsible for handling HTTP requests related to metadata assets.
    /// Exposes endpoints for creating and retrieving assets.
    /// </summary>
    [ApiController]
    [Route("api/metadata/assets")]
    public class MetadataAssetsController : ControllerBase
    {
        private readonly IMetadataAssetService _service;
        private readonly ILogger<MetadataAssetsController> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="MetadataAssetsController"/>.
        /// </summary>
        /// <param name="service">The metadata asset service for business logic and storage.</param>
        /// <param name="logger">The logger instance for structured logging.</param>
        public MetadataAssetsController(IMetadataAssetService service, ILogger<MetadataAssetsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new metadata asset.
        /// Returns 201 Created with the asset on success,
        /// or 409 Conflict if an asset with the same uid already exists.
        /// </summary>
        /// <param name="request">The request body containing asset details.</param>
        /// <returns>The created asset, or an error message if creation failed.</returns>
        [HttpPost]
        public IActionResult Create([FromBody] CreateMetadataAssetRequest request)
        {
            var asset = new MetadataAsset
            {
                Uid = request.Uid!,
                Type = request.Type!,
                Name = request.Name!,
                Description = request.Description,
                Owner = request.Owner,
                SourceSystem = request.SourceSystem!,
                Tags = request.Tags ?? new List<string>()
            };

            var (success, error, created) = _service.Create(asset);

            if (!success)
                return Conflict(new { error });

            return CreatedAtAction(nameof(GetByUid), new { uid = created!.Uid }, ToResponse(created));
        }

        /// <summary>
        /// Retrieves a metadata asset by its unique identifier.
        /// Returns 200 OK with the asset, or 404 Not Found if it does not exist.
        /// </summary>
        /// <param name="uid">The unique identifier of the asset to retrieve.</param>
        /// <returns>The matching asset, or a not found error message.</returns>
        [HttpGet("{uid}")]
        public IActionResult GetByUid(string uid)
        {
            var asset = _service.GetByUid(uid);

            if (asset is null)
                return NotFound(new { error = $"Asset with uid '{uid}' not found." });

            return Ok(ToResponse(asset));
        }

        /// <summary>
        /// Retrieves all metadata assets, with an optional filter by type.
        /// Always returns 200 OK — an empty list if no assets match.
        /// </summary>
        /// <param name="type">Optional asset type to filter by (e.g. DATA_PRODUCT, DATASET). Case-insensitive.</param>
        /// <returns>A list of matching assets.</returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] string? type = null)
        {
            var assets = _service.GetAll(type);
            return Ok(assets.Select(ToResponse));
        }

        /// <summary>
        /// Maps a <see cref="MetadataAsset"/> domain model to a <see cref="MetadataAssetResponse"/> DTO.
        /// </summary>
        /// <param name="asset">The internal domain model to map.</param>
        /// <returns>A response DTO safe to expose via the API.</returns>
        private static MetadataAssetResponse ToResponse(MetadataAsset asset) => new()
        {
            Uid = asset.Uid,
            Type = asset.Type,
            Name = asset.Name,
            Description = asset.Description,
            Owner = asset.Owner,
            SourceSystem = asset.SourceSystem,
            Tags = asset.Tags
        };

    }
}
