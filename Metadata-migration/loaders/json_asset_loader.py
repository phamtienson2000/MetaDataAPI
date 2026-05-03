import json
from dataclasses import asdict
from pathlib import Path
from typing import List

from models.metadata_asset import MetadataAsset
from models.invalid_asset import InvalidAsset


class JsonAssetLoader:

    def __init__(self, valid_path: str, invalid_path: str):
        self.valid_path = Path(valid_path)
        self.invalid_path = Path(invalid_path)

    def load(self, valid_assets: List[MetadataAsset], invalid_assets: List[InvalidAsset]) -> None:
        # Create Folder if not exist
        self.valid_path.parent.mkdir(parents=True, exist_ok=True)
        self.invalid_path.parent.mkdir(parents=True, exist_ok=True)

        self._write_json(
            self.valid_path,
            [asdict(asset) for asset in valid_assets]
        )
        self._write_json(
            self.invalid_path,
            [{"raw": asset.raw, "errors": asset.errors} for asset in invalid_assets]
        )

    def _write_json(self, path: Path, data: list) -> None:
        with path.open("w", encoding="utf-8") as f:
            json.dump(data, f, indent=2, ensure_ascii=False)
        print(f"  → Written {len(data)} record(s) to {path}")
