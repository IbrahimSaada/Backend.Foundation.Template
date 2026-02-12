# Backend.Foundation.Template

Clean Architecture .NET backend template focused on reusable structure, strict layer boundaries, and provider-agnostic application logic.

## Documentation
Architecture and usage guide:
- `docs/ARCHITECTURE_GUIDE.md`

## Projects
- `Backend.Foundation.Template` - Host/API
- `Backend.Foundation.Template.Application` - use-cases, handlers, behaviors
- `Backend.Foundation.Template.Domain` - domain model and core rules
- `Backend.Foundation.Template.Abstractions` - contracts (`IRepository`, `IUnitOfWork`, results, paging)
- `Backend.Foundation.Template.GenericRepo` - generic EF/Mongo repository implementations
- `Backend.Foundation.Template.Persistence` - DbContext, migrations, provider wiring

## Quick Start
1. Set persistence provider and connection string in `appsettings.*.json`.
2. Apply migration:
```bash
dotnet ef database update \
  --configuration Release \
  --project Backend.Foundation.Template.Persistence \
  --startup-project Backend.Foundation.Template \
  --context AppDbContext
```
3. Run API:
```bash
dotnet run --project Backend.Foundation.Template
```

## Notes
- API sample requests are in `Backend.Foundation.Template/Backend.Foundation.Template.http`.
- Global architecture rules and workflow diagrams are in `docs/ARCHITECTURE_GUIDE.md`.
