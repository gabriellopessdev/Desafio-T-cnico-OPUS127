# ADR 0001 — Backend e frontend na raiz; camadas num único projeto de API

**Status:** Aceito  
**Data:** 2026-09-16

## Contexto

Serviço .NET e interface React são ciclos de vida diferentes (SDK, pacotes, testes). Juntar os dois sob um único `src/` na raiz do repositório mistura mundos. O modelo clássico de Clean Architecture ainda parte a API em vários projetos (Domínio, Infraestrutura), o que é cerimônia demais para um serviço com uma entidade e duas rotas.

## Decisão

```
backend/          solution .NET, src e testes
  src/Api/        um .csproj (Controllers, Services, Repositories, …)
  tests/
  Opus127.Dengue.sln
frontend/         React (src/, package.json)
docs/
docker-compose.yml
README.md
```

Pastas de camada **dentro** de `backend/src/Api`. Sem `Domain.csproj` / `Infrastructure.csproj` por enquanto. Sem MediatR.

## Consequências

- Quem abre o repositório vê backend e frontend na hora.
- `dotnet test` e a solution ficam isolados do Node.
- Se o domínio crescer (várias entidades, mais origens), aí sim extrair projetos de domínio e infraestrutura **dentro** de `backend/src/`.
