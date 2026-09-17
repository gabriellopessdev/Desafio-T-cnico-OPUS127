# ADR 0006 — Testes de integração no SQL Server real

**Status:** Aceito  
**Data:** 2026-09-16

## Contexto

EF InMemory não executa o mesmo SQL (índice único, tipos). O desafio pede teste unitário e/ou de integração; a persistência é o coração da Q1/Q2.

## Decisão

Unitários sem I/O para calendário, mapeamento e sync com HTTP fake. Integração com `WebApplicationFactory` + Testcontainers.MsSql (SQL Server de verdade). Sem suíte E2E do React no MVP.

## Consequências

- Confiança maior no GET/upsert.
- Máquina (e CI, se houver) precisa de Docker para a suíte completa.
- xUnit + FluentAssertions + NSubstitute.
