from commands.base_command import BaseCommand
from services.metadata_migration_service import MetadataMigrationService


class MigrateAssetsCommand(BaseCommand):

    def __init__(self, service: MetadataMigrationService):
        self.service = service

    def execute(self) -> None:
        self.service.run()
