import json
from pathlib import Path
from typing import List


class JsonAssetExtractor:
    def __init__(self, file_path: str):
        self.file_path = Path(file_path)

    def extract(self) -> List[dict]:
        if not self.file_path.exists():
            raise FileNotFoundError(f"Input file not found: {self.file_path}")
        with self.file_path.open(encoding="utf-8") as f:
            return json.load(f)
