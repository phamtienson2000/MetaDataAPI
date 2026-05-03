"""
ServiceFactory — nơi DUY NHẤT lắp ráp (wire up) tất cả các dependency.

First principle: "Factory giải quyết bài toán: Ai tạo ra ai?"
  Nếu không có Factory, main.py phải biết về tất cả các class
  → vi phạm principle "code phụ thuộc vào abstraction, không phụ thuộc vào detail."

Factory Pattern lợi ích:
  - main.py chỉ cần gọi ServiceFactory.create_migration_service()
  - Muốn đổi input file path? Chỉ sửa ở đây.
  - Muốn dùng MockLoader trong test? Tạo TestServiceFactory kế thừa.

Cấu trúc dependency (đọc từ trong ra ngoài):
  Extractor ──┐
  Mapper   ──→ Processor ──→ Service
  Validator──┘
  Loader   ──┘
"""
from extractors.json_asset_extractor import JsonAssetExtractor
from mappers.asset_mapper import AssetMapper
from validators.asset_validator import AssetValidator
from loaders.json_asset_loader import JsonAssetLoader
from processors.metadata_migration_processor import MetadataMigrationProcessor
from services.metadata_migration_service import MetadataMigrationService


class ServiceFactory:

    @staticmethod
    def create_migration_service() -> MetadataMigrationService:
        extractor  = JsonAssetExtractor(file_path="input/raw_assets.json")
        mapper     = AssetMapper()
        validator  = AssetValidator()
        loader     = JsonAssetLoader(
            valid_path="output/valid_assets.json",
            invalid_path="output/invalid_assets.json",
        )
        processor = MetadataMigrationProcessor(
            extractor=extractor,
            mapper=mapper,
            validator=validator,
            loader=loader,
        )
        return MetadataMigrationService(processor=processor)
