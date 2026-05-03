import sys
from factories.service_factory import ServiceFactory
from commands.migrate_assets_command import MigrateAssetsCommand


#  COMMAND REGISTRY
COMMAND_REGISTRY = {
    "migrate-assets": lambda: MigrateAssetsCommand(
        service=ServiceFactory.create_migration_service()
    ),
}


def main() -> None:
    if len(sys.argv) < 2:
        _print_usage()
        sys.exit(1)

    command_name = sys.argv[1]

    if command_name not in COMMAND_REGISTRY:
        print(f"❌ Unknown command: '{command_name}'")
        _print_usage()
        sys.exit(1)

    # Create command instance and Run
    command = COMMAND_REGISTRY[command_name]()
    command.execute()


def _print_usage() -> None:
    print("Usage: python main.py <command>")
    print("Available commands:")
    for name in COMMAND_REGISTRY:
        print(f"  {name}")


if __name__ == "__main__":
    main()
