using MetadataApi.Models;

namespace MetadataApi.Services
{
    /// <summary>
    /// In-memory implementation of <see cref="IMetadataAssetService"/>.
    /// Assets are stored in a <see cref="Dictionary{TKey, TValue}"/> keyed by uid
    /// for O(1) lookup performance. Data does not persist between application restarts.
    /// </summary>
    public class MetadataAssetService: IMetadataAssetService
    {
        private readonly Dictionary<string, MetadataAsset> _store = new();
        private readonly ILogger<MetadataAssetService> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="MetadataAssetService"/>.
        /// </summary>
        /// <param name="logger">The logger instance for structured logging.</param>
        public MetadataAssetService(ILogger<MetadataAssetService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a metadata asset by its unique identifier.
        /// Uses dictionary lookup for O(1) performance.
        /// </summary>
        /// <param name="uid">The unique identifier of the asset to retrieve.</param>
        /// <returns>The matching <see cref="MetadataAsset"/>, or null if not found.</returns>
        public MetadataAsset? GetByUid(string uid)
        {
            _store.TryGetValue(uid, out var asset);
            return asset;
        }

        /// <summary>
        /// Retrieves all metadata assets, with an optional filter by type.
        /// Type comparison is case-insensitive.
        /// </summary>
        /// <param name="type">Optional asset type to filter by (e.g. DATA_PRODUCT). Returns all assets if null or empty.</param>
        /// <returns>A collection of matching <see cref="MetadataAsset"/> objects.</returns>
        public IEnumerable<MetadataAsset> GetAll(string? type = null)
        {
            var assets = _store.Values.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(type))
                assets = assets.Where(a => a.Type.Equals(type, StringComparison.OrdinalIgnoreCase));

            return assets;
        }

        /// <summary>
        /// Creates and stores a new metadata asset.
        /// Returns a failure result if an asset with the same uid already exists.
        /// </summary>
        /// <param name="asset">The <see cref="MetadataAsset"/> to create.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        ///   <item><description><c>Success</c>: true if the asset was stored successfully.</description></item>
        ///   <item><description><c>Error</c>: a conflict message if the uid already exists, otherwise null.</description></item>
        ///   <item><description><c>Asset</c>: the stored asset if successful, otherwise null.</description></item>
        /// </list>
        /// </returns>
        public (bool Success, string? Error, MetadataAsset? Asset) Create(MetadataAsset asset)
        {
            if (_store.ContainsKey(asset.Uid))
            {
                _logger.LogWarning("Duplicate asset uid attempted: {Uid}", asset.Uid);
                return (false, $"An asset with uid '{asset.Uid}' already exists.", null);
            }

            _store[asset.Uid] = asset;
            _logger.LogInformation("Asset created with uid: {Uid}", asset.Uid);
            return (true, null, asset);
        }
    }
}
