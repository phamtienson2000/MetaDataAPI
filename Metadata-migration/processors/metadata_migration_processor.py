from typing import Tuple, List

from models.metadata_asset import MetadataAsset
from models.invalid_asset import InvalidAsset
from extractors.json_asset_extractor import JsonAssetExtractor
from mappers.asset_mapper import AssetMapper
from validators.asset_validator import AssetValidator
from loaders.json_asset_loader import JsonAssetLoader


class MetadataMigrationProcessor:

    def __init__(
        self,
        extractor: JsonAssetExtractor,
        mapper: AssetMapper,
        validator: AssetValidator,
        loader: JsonAssetLoader,
    ):
        self.extractor = extractor
        self.mapper = mapper
        self.validator = validator
        self.loader = loader

    def process(self) -> Tuple[List[MetadataAsset], List[InvalidAsset]]:
        raw_assets = self.extractor.extract()
        print(f"  Extracted {len(raw_assets)} raw asset(s)")

        valid_assets: List[MetadataAsset] = []
        invalid_assets: List[InvalidAsset] = []

        for raw in raw_assets:
            # Step 1: Normalize
            asset = self.mapper.map(raw)

            # Step 2: Validate
            errors = self.validator.validate(asset)

            # Step 3: Categorize
            if errors:
                invalid_assets.append(InvalidAsset(raw=raw, errors=errors))
            else:
                valid_assets.append(asset)

        # Step 4: Output valid-invalid results
        self.loader.load(valid_assets, invalid_assets)

        return valid_assets, invalid_assets
