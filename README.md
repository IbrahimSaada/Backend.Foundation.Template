# Backend.Foundation.Template

Clean Architecture .NET backend template focused on reusable structure, strict layer boundaries, and provider-agnostic application logic.

## Documentation
Architecture and usage guide:
- `docs/ARCHITECTURE_GUIDE.md`
  - includes Keycloak setup flow (bootstrap admin hardening, realm/client/roles/users, backend config, smoke tests)
  - includes Redis phase 1/2 (infrastructure foundation + application query caching/invalidation pipeline)

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

## Local Keycloak (Reusable)
- Compose file: `docker/keycloak/docker-compose.yml`
- Linux portability is included via `extra_hosts: host.docker.internal:host-gateway`.
- Start:
```bash
docker compose -f docker/keycloak/docker-compose.yml up -d
```
- Stop:
```bash
docker compose -f docker/keycloak/docker-compose.yml down
```
- Security note:
  - `KC_BOOTSTRAP_ADMIN_USERNAME` and `KC_BOOTSTRAP_ADMIN_PASSWORD` are only for first startup.
  - After creating a permanent admin user, remove those two variables and recreate the container.

## Local Redis (Reusable)
- Compose file: `docker/redis/docker-compose.yml`
- Start:
```bash
docker compose -f docker/redis/docker-compose.yml up -d
```
- Stop:
```bash
docker compose -f docker/redis/docker-compose.yml down
```
- App config:
  - enable Redis in `Backend.Foundation.Template/appsettings.*.json` with the `Redis` section.
  - default local connection string is `localhost:6379`.

## Health Endpoints
- Liveness: `GET /health/live`
- Readiness: `GET /health/ready`
- Redis readiness is part of `/health/ready` when Redis is enabled.

## Notes
- API sample requests are in `Backend.Foundation.Template/Backend.Foundation.Template.http`.
- In `Authentication:Enabled=false`, template uses an authenticated dev principal for local DX.
- Global architecture rules and workflow diagrams are in `docs/ARCHITECTURE_GUIDE.md`.
