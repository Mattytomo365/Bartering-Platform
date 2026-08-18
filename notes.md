# Config Files

## .env
- A simple key=value file that Docker Compose auto-loads.
- Purpose: keep secrets and per-machine settings out of Git while still injecting them into containers.

## ocelot.development.json
The routing config for your API Gateway (Ocelot).

It tells Ocelot:
- Upstream: what the client calls (e.g., /api/listings/...).
- Downstream: where to proxy the request (host+port of the microservice).
- Auth scheme: which authentication handler to use ("Bearer" in your case).
- BaseUrl: the public URL of the gateway (used in some scenarios, e.g., service discovery, swagger aggregators).

---

# Docker integration

## docker-compose.yml:
- Defines your multi-container app: SQL Server, RabbitMQ, your services, the gateway, and the Angular app.
- Purpose: bring everything up/down with one command; pass env vars into services; define networks/volumes/ports.
- Your .NET services read secrets from environment variables (e.g., ConnectionStrings__Sql) that Compose sets.

docker-compose.yml is a living diagram: which services exist, how they talk, ports, volumes, healthchecks. It communicates system design at a glance.

## ocelot.docker.json
Tells Ocelot (your API Gateway) how to proxy requests to the microservices inside the Docker network. So the Downstream Host must be the Compose service name, and the Port/Scheme must match what the service listens on inside its container

## /deploy folder
- Before it: install SQL Server + RabbitMQ locally, wire ports by hand, tweak config files, risk secrets in Git, inconsistent versions between machines.
- After it: infra-as-code, 1 command to run, secrets injected via env, deterministic versions, obvious topology. Shows you understand modern devops basics.