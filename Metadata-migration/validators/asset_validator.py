from typing import List

from models.metadata_asset import MetadataAsset


REQUIRED_FIELDS = ["uid", "type", "name"]


class AssetValidator:

    def validate(self, asset: MetadataAsset) -> List[str]:
        errors = []
        for field_name in REQUIRED_FIELDS:
            value = getattr(asset, field_name)
            if not value:  
                errors.append(f"Missing required field: '{field_name}'")
        return errors
