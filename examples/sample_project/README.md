# Sample pbt Project

This is an example project demonstrating the structure and features of pbt (Power BI Build Tool).

## Project Structure

```
sample_project/
├── tables/              # Table definitions (reusable across models)
│   ├── sales.yaml       # Fact table with partitions and calculated columns
│   ├── customers.yaml   # Dimension table with hierarchy and data categories
│   └── datedim.yaml     # Date dimension with sort-by-column support
├── models/              # Model compositions
│   └── sales_model.yaml # Full-featured model with relationships, measures, etc.
├── environments/        # Named environment overrides (optional)
│   └── dev.env.yml      # Development connection string overrides
├── target/              # Generated TMDL/PBIP output (created by build)
└── .pbt/                # Lineage tag manifest (auto-generated)
```

## Files

### models/sales_model.yaml

Each model file is self-contained and carries all configuration:

- `name` / `description` — semantic model identity
- `compatibility_level` — Power BI dataset compatibility level (default: 1600)
- `format_strings` — optional type-level default format strings
- `assets` — optional override for table/macro lookup paths (defaults to `tables/` and `macros/` next to the project root)
- `builds` — optional build output path override
- `tables` — references to table definitions in the registry
- `relationships` — table-to-table relationships with cardinality and cross-filter settings
- `measures` — DAX measures with display folders
- `expressions` — shared Power Query parameters (e.g., connection strings)
- `calculation_groups` — time intelligence and other calculation groups
- `perspectives` — scoped visibility for different audiences
- `roles` — row-level security (RLS) definitions
- `field_parameters` — dynamic axis switching for reports

### tables/

Contains reusable table definitions that can be referenced by multiple models:

- **sales.yaml** — Sales fact table demonstrating:
  - Multiple partitions (`Sales_Historical`, `Sales_Current`)
  - Calculated columns (`IsLargeOrder`)
  - Table-level measures with display folders
  - Column properties: `is_key`, `is_hidden`, `summarize_by`, `source_column`

- **customers.yaml** — Customer dimension table demonstrating:
  - Geography hierarchy (Region > Country > City)
  - Data categories (`Organization`, `City`, `Country`)
  - Annotations (`PBI_GeoEncoding`)

- **datedim.yaml** — Date dimension table demonstrating:
  - `sort_by_column` (MonthName sorted by MonthNum)
  - Hidden helper columns (`MonthNum`)

### environments/

- **dev.env.yml** — Overrides `ServerName` and `DatabaseName` expressions for the development environment. Use with `pbt build --env dev`.

## Usage

```bash
# Validate the project
pbt validate ./sample_project

# Build full Power BI project (.pbip) from YAML
pbt build ./sample_project

# Build TMDL-only output (no .pbip wrapper)
pbt build model ./sample_project

# Build with dev environment overrides
pbt build ./sample_project --env dev

# Dry run — validate and compose without writing files
pbt build ./sample_project --dry-run

# List tables and models
pbt list ./sample_project
pbt list ./sample_project --details

# View lineage tags
pbt lineage show ./sample_project
pbt lineage show ./sample_project --details

# Compare two project snapshots for breaking changes
pbt diff ./sample_project_v1 ./sample_project_v2
```

## Generated Output

After running `pbt build`, the `target/` directory will contain a full PBIP structure:

```
target/
└── SalesAnalytics/
    ├── SalesAnalytics.pbip
    └── SalesAnalytics.SemanticModel/
        └── definition/
            ├── database.tmdl
            ├── model.tmdl
            └── tables/
                ├── Sales.tmdl
                ├── Customers.tmdl
                └── DateDim.tmdl
```

This PBIP can be opened in Power BI Desktop or deployed to Power BI Service.
