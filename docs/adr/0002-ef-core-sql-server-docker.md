# ADR 0002 — EF Core + SQL Server no Docker (apps na máquina)

**Status:** Aceito  
**Data:** 2026-09-16

## Contexto

Persistência obrigatória: SQL Server com ORM. Empacotar API e React em imagens .NET 10 aumenta risco de onboarding. Instalar SQL Server “nu” no Windows do avaliador é frágil.

## Decisão

Entity Framework Core contra SQL Server 2022 no `docker-compose.yml`. API (`dotnet run`) e SPA (`npm run dev`) na máquina. Migrations no projeto da API.

## Consequências

- README previsível: Compose + duas ferramentas de dev.
- ORM canônico Microsoft, alinhado ao anúncio de “ORM”.
- Quem avaliar precisa de Docker só para o banco, não de SDK na imagem.
