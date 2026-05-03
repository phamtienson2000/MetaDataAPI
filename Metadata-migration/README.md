# Metadata Migration

A Python ETL pipeline that reads raw metadata assets from a JSON source, normalizes and validates them, then separates valid and invalid records into distinct output files.

---
## Estimated Time Spent

~14 hours — architecture design (2h), implementation and testing (10h), documentation (2h).

---

## Table of Contents

- [Overview](#overview)
- [Project Structure](#project-structure)
- [How It Works](#how-it-works)
- [How To Run](#how-to-run)
- [Input Format](#input-format)
- [Output Format](#output-format)
- [Normalization Rules](#normalization-rules)
- [Validation Rules](#validation-rules)
- [How to Extend](#how-to-extend)

---

## Overview

This project simulates a real-world metadata migration scenario: a source system exports assets as JSON, and we need to clean, validate, and load them into a target system.

The pipeline follows the **ETL pattern** (Extract → Transform → Load) and is structured using clean architecture principles — each layer has a single responsibility and can be swapped or extended independently.

---

## Project Structure

```
metadata-migration/
│
├── main.py                            # Entry point — parses CLI command
│
├── commands/
│   ├── base_command.py                # Abstract base class for all commands
│   └── migrate_assets_command.py      # Command that triggers the migration
│
├── factories/
│   └── service_factory.py             # Wires up all dependencies
│
├── services/
│   └── metadata_migration_service.py  # High-level business logic + reporting
│
├── processors/
│   └── metadata_migration_processor.py # Orchestrates the ETL pipeline
│
├── extractors/
│   └── json_asset_extractor.py        # Reads raw JSON from file
│
├── mappers/
│   └── asset_mapper.py                # Normalizes raw dict → MetadataAsset
│
├── validators/
│   └── asset_validator.py             # Validates required fields
│
├── loaders/
│   └── json_asset_loader.py           # Writes valid/invalid output files
│
├── models/
│   ├── metadata_asset.py              # Clean internal data model
│   └── invalid_asset.py               # Model for rejected assets + errors
│
├── input/
│   └── raw_assets.json                # Source data (raw, uncleaned)
│
└── output/
    ├── valid_assets.json              # Generated — assets that passed validation
    └── invalid_assets.json            # Generated — assets that failed + reasons
```

---

## How It Works

```
python main.py migrate-assets
       │
       ▼
  COMMAND REGISTRY
  resolves "migrate-assets"
       │
       ▼
  MigrateAssetsCommand.execute()
       │
       ▼
  MetadataMigrationService.run()
       │
       ▼
  MetadataMigrationProcessor.process()
       │
       ├── JsonAssetExtractor   →  reads raw JSON
       ├── AssetMapper          →  normalizes each record
       ├── AssetValidator       →  checks required fields
       └── JsonAssetLoader      →  writes output files
```

Each layer only knows about its immediate neighbors. Swapping any layer (e.g., reading from an API instead of a file) requires changing only that one class.

---

## Getting Started

**Requirements:** Python 3.8+. No external dependencies — only the standard library.

**Clone or set up the project:**

```bash
cd metadata-migration
```

**Run the migration:**

```bash
python main.py migrate-assets
```

**Expected output:**

```
▶ Starting metadata migration...

  Extracted 3 raw asset(s)
  → Written 2 record(s) to output/valid_assets.json
  → Written 1 record(s) to output/invalid_assets.json

────────────────────────────────────────
  ✓ Valid assets   : 2
  ✗ Invalid assets : 1

  Invalid asset details:
    • [(no uid)] (no name) → Missing required field: 'uid', Missing required field: 'name'
────────────────────────────────────────

 Migration complete.
```

---

## Input Format

Place your source file at `input/raw_assets.json`. Each asset is a JSON object in an array.

```json
[
  {
    "uid": "asset-001",
    "type": "DATA_PRODUCT",
    "name": "Customer Orders",
    "description": "<p>Contains fictional customer order information</p>",
    "owner": "team-alpha",
    "createdOn": "2025-01-10T12:30:00Z",
    "updatedOn": "2025-02-15T09:00:00Z",
    "tags": ["certified", "sales"]
  }
]
```

| Field | Type | Notes |
|---|---|---|
| `uid` | string | Required. Empty string treated as missing. |
| `type` | string | Required. |
| `name` | string | Required. |
| `description` | string / null | Optional. HTML tags will be stripped. |
| `owner` | string / null | Optional. Empty string treated as null. |
| `createdOn` | string / null | ISO 8601. Invalid dates are set to null. |
| `updatedOn` | string / null | ISO 8601. Invalid dates are set to null. |
| `tags` | array / null | Optional. Null is normalized to empty list. |

---

## Output Format

**`output/valid_assets.json`** — assets that passed all validation rules:

```json
[
  {
    "uid": "asset-001",
    "type": "DATA_PRODUCT",
    "name": "Customer Orders",
    "description": "Contains fictional customer order information",
    "owner": "team-alpha",
    "created_on": "2025-01-10T12:30:00+00:00",
    "updated_on": "2025-02-15T09:00:00+00:00",
    "tags": ["certified", "sales"]
  }
]
```

**`output/invalid_assets.json`** — assets that failed validation, with the original raw data and error messages:

```json
[
  {
    "raw": {
      "uid": "",
      "type": "BUSINESS_TERM",
      "name": null,
      ...
    },
    "errors": [
      "Missing required field: 'uid'",
      "Missing required field: 'name'"
    ]
  }
]
```

---

## Normalization Rules

Applied by `AssetMapper` before validation:

| Rule | Input example | Output |
|---|---|---|
| Strip HTML tags from description | `<p>Hello</p>` | `Hello` |
| Empty strings → None | `""` / `"   "` | `null` |
| Valid dates → ISO 8601 string | `"2025-01-10T12:30:00Z"` | `"2025-01-10T12:30:00+00:00"` |
| Invalid dates → None | `"invalid-date"` | `null` |
| Null tags → empty list | `null` | `[]` |

---

## Validation Rules

Applied by `AssetValidator` after normalization. An asset is **invalid** if any of these fields are missing or null:

- `uid`
- `type`
- `name`

All errors are collected before rejecting — a single asset can have multiple error messages.

---
## Assumptions

- **Empty string treated as missing** — `uid`, `type`, and `name` with value `""` are considered absent and will fail validation. An empty string cannot serve as a meaningful identifier.
- **Invalid dates are nulled, not rejected** — if `createdOn` or `updatedOn` cannot be parsed, the field is set to `null` rather than rejecting the whole asset. Date is not a required field.
- **`tags: null` normalized to `[]`** — null tags is treated as an empty list, not an error. Downstream code expects a list.
- **Output files are overwritten on each run** — existing output files are fully replaced, not appended to.

---

## Questions I Would Ask

1. Should an invalid date reject the whole asset, or is nulling the field sufficient?
2. Is `type` a fixed set of allowed values (`DATA_PRODUCT`, `DATASET`, `BUSINESS_TERM`) or any free-form string?
3. Should output files append or overwrite when the pipeline runs multiple times?


