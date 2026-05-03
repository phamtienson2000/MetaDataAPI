import re
from datetime import datetime, timezone
from typing import Optional, List

from models.metadata_asset import MetadataAsset


class AssetMapper:

    def map(self, raw: dict) -> MetadataAsset:
        """Nhận 1 raw dict, trả về 1 MetadataAsset đã normalize."""
        return MetadataAsset(
            uid=self._clean_str(raw.get("uid")),
            type=self._clean_str(raw.get("type")),
            name=self._clean_str(raw.get("name")),
            description=self._strip_html(raw.get("description")),
            owner=self._clean_str(raw.get("owner")),
            created_on=self._parse_date(raw.get("createdOn")),
            updated_on=self._parse_date(raw.get("updatedOn")),
            tags=self._normalize_tags(raw.get("tags")),
        )

    # ------------------------------------------------------------------ #
    #  Private helpers          
    # ------------------------------------------------------------------ #

    def _clean_str(self, value) -> Optional[str]:
        if value is None:
            return None
        stripped = str(value).strip()
        return stripped if stripped else None

    def _strip_html(self, value) -> Optional[str]: 
        if value is None:
            return None
        no_html = re.sub(r"<[^>]+>", "", value)
        return self._clean_str(no_html)

    def _parse_date(self, value) -> Optional[str]:
        if value is None:
            return None
        try:
            normalized = value.replace("Z", "+00:00")
            dt = datetime.fromisoformat(normalized)
            return dt.isoformat()
        except (ValueError, AttributeError):
            return None

    def _normalize_tags(self, value) -> List[str]:
        if value is None:
            return []
        return list(value)
