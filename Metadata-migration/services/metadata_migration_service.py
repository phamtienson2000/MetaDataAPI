from processors.metadata_migration_processor import MetadataMigrationProcessor


class MetadataMigrationService:

    def __init__(self, processor: MetadataMigrationProcessor):
        self.processor = processor

    def run(self) -> None:
        print("\n▶ Starting metadata migration...\n")
        valid, invalid = self.processor.process()

        # Summary report
        print(f"\n{'─' * 40}")
        print(f"  ✓ Valid assets   : {len(valid)}")
        print(f"  ✗ Invalid assets : {len(invalid)}")
        if invalid:
            print("\n  Invalid asset details:")
            for item in invalid:
                uid = item.raw.get("uid") or "(no uid)"
                name = item.raw.get("name") or "(no name)"
                print(f"    • [{uid}] {name} → {', '.join(item.errors)}")
        print(f"{'─' * 40}")
        print("\n Migration complete.\n")
