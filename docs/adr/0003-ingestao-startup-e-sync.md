# ADR 0003 — Ingestão no startup e via POST /sync

**Status:** Aceito  
**Data:** 2026-09-16

## Contexto

A Questão 1 pede consultar e persistir. Só no boot é opaco na demo; só endpoint exige um passo extra que o avaliador pode não ler.

## Decisão

`IHostedService` dispara a sync uma vez após o start (exceções logadas; o host não cai). `POST /api/dengue/sync` reprocessa. Persistência é upsert por (ano, semana). Cliente HTTP via `IHttpClientFactory`. Falha da AlertaDengue não impede o GET no que já existe.

## Consequências

- Primeira subida já enche o banco (se a Infodengue responder).
- Demo e testes conseguem reexecutar ingestão.
- Startup não pode bloquear o Kestrel indefinidamente: sync em background.
