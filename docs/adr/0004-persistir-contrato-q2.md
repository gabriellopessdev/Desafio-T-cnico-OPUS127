# ADR 0004 — Persistir só o contrato da Questão 2

**Status:** Aceito  
**Data:** 2026-09-16

## Contexto

O dicionário Infodengue tem dezenas de campos. Persistir tudo demonstra leitura do PDF, mas vira dump e foge do que a Q2 realmente usa.

## Decisão

Tabela mínima: ano, semana, casos estimados, casos notificados, nível de alerta, geocode e timestamp de sync. O cliente HTTP pode desserializar só o necessário do JSON de origem.

## Consequências

- Modelo e migrations simples; alinhado ao recorte A do brainstorm.
- Não dá para explorar Rt/incidência na UI sem mudar o schema (aceitável no MVP).
