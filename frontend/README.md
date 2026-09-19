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

Abre `http://localhost:5173` (porta fixa; se estiver ocupada o Vite falha em vez de ir para `5174`). A origem da API é `VITE_API_URL` em `.env.development` (padrão `http://localhost:5080`). Reinicie o Vite depois de editar.

## O que a tela faz

- Calcula as três últimas semanas fechadas no fuso `America/Sao_Paulo` (a vigente fica de fora).
- Dispara três `GET /api/dengue?ew=&ey=` em paralelo.
- 404 vira cartão “Sem registro”; falha de rede esconde dados parciais.

O guia completo (pré-requisitos, configuração, Docker, API e testes) está no [README da raiz](../README.md).
