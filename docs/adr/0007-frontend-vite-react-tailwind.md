# ADR 0007 — SPA Vite + React + TypeScript + Tailwind

**Status:** Aceito  
**Data:** 2026-09-16

## Contexto

O PDF pede tecnologias web + framework React, três chamadas ao endpoint da Q2, UI intuitiva (tabela, gráfico ou cards). Next.js adiciona SSR que o enunciado não pede.

## Decisão

Aplicação em `frontend/` (Vite + React + TypeScript + Tailwind; código em `frontend/src`). Uma tela. Três GET em paralelo para as 3 últimas semanas epidemiológicas. Cards (cor por nível 1–4) + tabela + gráfico Recharts. `VITE_API_URL`. Sem roteador além do necessário, sem auth.

## Consequências

- Cumpre Q3 literalmente (três requisições).
- CORS na API de dev é obrigatório.
- 404 de uma semana vira estado vazio, não tela branca.
