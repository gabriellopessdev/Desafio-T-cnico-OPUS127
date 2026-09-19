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

Abre `http://localhost:5173`. A origem da API vem de `VITE_API_URL` em `.env.development` (`http://localhost:5080`).

## O que a tela faz

- Calcula as três últimas semanas fechadas no fuso `America/Sao_Paulo` (a vigente fica de fora).
- Dispara três `GET /api/dengue?ew=&ey=` em paralelo.
- 404 vira cartão “Sem registro”; falha de rede esconde dados parciais.

O guia completo (Docker, API, testes) permanece no README da raiz — Task 9 do plano.
