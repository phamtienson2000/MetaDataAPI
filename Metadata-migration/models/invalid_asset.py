from dataclasses import dataclass, field
from typing import List

@dataclass
class InvalidAsset:
    raw: dict               # Raw Data
    errors: List[str]       # Reject
