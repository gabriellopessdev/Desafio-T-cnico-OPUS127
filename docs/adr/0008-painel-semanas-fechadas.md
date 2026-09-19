# ADR 0008 — Painel só com semanas epidemiológicas fechadas

**Status:** Aceito  
**Data:** 2026-09-19

## Contexto

A Infodengue publica o boletim depois que a semana (domingo a sábado) termina. Incluir a semana vigente no painel gerava um cartão “Sem registro” (404) quase sempre, sem valor de vigilância. As três leituras da Q3 devem mostrar o recorte mais recente **já encerrado**.

## Decisão

- `lastWeeks` / `LastWeeks` começam no domingo da semana **anterior** à data de “hoje” (`America/Sao_Paulo` no React; o calendário do serviço usa a mesma regra Sunday/CDC).
- A semana em curso nunca entra nas três consultas do painel.
- A tela informa isso de forma explícita (texto no cabeçalho e recado acima dos cartões).
- A carga de seis meses (Q1) continua até a data atual: a origem pode já devolver a semana vigente; isso não muda o recorte da interface.

## Consequências

- Em 19/09/2026 (SE 2026-37 ainda aberta), o painel pede 2026-36, 2026-35 e 2026-34.
- 404 ainda é possível numa semana fechada se o boletim atrasar; o cartão vazio permanece para esse caso.
- Quem ler “três últimas semanas” no enunciado deve interpretar “três últimas **fechadas**”.
