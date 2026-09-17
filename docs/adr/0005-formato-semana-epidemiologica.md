# ADR 0005 — `SE` YYYYWW na origem vs `"YYYY-WW"` na API

**Status:** Aceito  
**Data:** 2026-09-16

## Contexto

A AlertaDengue devolve `SE` como inteiro (`202602`). O exemplo do desafio usa `"2023-40"`. A query da Q2 é `ew` + `ey` separados. A Infodengue trata `data_iniSE` como domingo.

## Decisão

- Banco: `EpidemiologicalYear` + `EpidemiologicalWeek`.
- HTTP de leitura: `semana_epidemiologica` = `$"{year}-{week:D2}"`.
- Calendário: semana começa no domingo; semana 1 contém 4 de janeiro (CDC / Infodengue). “Hoje” em **America/Sao_Paulo**. Janela de 6 meses civis; se cruzar o ano, duas chamadas (`ew_end=53` no ano anterior é aceitável).

## Consequências

- Contrato do PDF cumprido sem mentir sobre a origem.
- Testes devem fixar um `SE` real conhecido (ex.: `202601` com início em 2026-01-04).
