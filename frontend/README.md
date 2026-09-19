# Painel de dengue — Belo Horizonte

Interface React (Vite + TypeScript + Tailwind) da Q3: três consultas em paralelo a `GET /api/dengue` para as últimas semanas epidemiológicas já encerradas.

## Pré-requisitos

- Node 20+
- API local em `http://localhost:5080` (veja o [README da raiz](../README.md))

## Execução

```powershell
cd frontend
npm install
npm run dev
```

Abre `http://localhost:5173`. A origem da API é `VITE_API_URL` (padrão `http://localhost:5080`). Para sobrescrever: copie `.env.example` para `.env.development` e reinicie o Vite.

## O que a tela faz

- Calcula as três últimas semanas fechadas no fuso `America/Sao_Paulo` (a vigente fica de fora).
- Dispara três `GET /api/dengue?ew=&ey=` em paralelo.
- 404 vira cartão “Sem registro”; falha de rede esconde dados parciais.

O guia completo (pré-requisitos, configuração, Docker, API e testes) está no [README da raiz](../README.md).
