# Alertas de dengue — Belo Horizonte

Serviço .NET e interface React para consultar boletins da Infodengue em Belo Horizonte (código IBGE `3106200`).

A API busca as últimas ~26 semanas (janela de seis meses civis) na [AlertaDengue](https://info.dengue.mat.br/), grava no SQL Server e responde consultas por semana e ano epidemiológicos. A interface mostra as **três últimas semanas já encerradas** (a vigente fica de fora), cada uma com um `GET` próprio.

Operação prevista: **banco no Docker**, **API e frontend na sua máquina**. Sem publicação em nuvem nesta versão.

## O que é obrigatório para rodar

Sem estes três itens a subida local **não funciona**:

| Ferramenta | Versão | Para quê |
|---|---|---|
| [Docker Desktop](https://docs.docker.com/get-docker/) (ou Engine + Compose) | com o plugin `compose` | SQL Server 2022 na porta `1433` |
| [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) | `10.0.x` (`dotnet --version`) | API e testes |
| [Node.js](https://nodejs.org/) | **20 ou superior** (`node -v`) | interface Vite |

Também é obrigatório:

1. **Docker Desktop aberto** (ícone da baleia) **antes** de `docker compose up` e **antes** de `dotnet test` (os testes de integração sobem outro SQL Server com Testcontainers).
2. **Portas livres:** `1433` (SQL), `5080` (API) e `5173` (Vite). No Windows, um SQL Server instalado na máquina costuma ocupar a `1433`.
3. **Acesso à internet na primeira sincronização**, para a API falar com `https://info.dengue.mat.br/`. Sem rede a API sobe mesmo assim; a carga falha no log e o `GET` só devolve o que já estiver no banco.
4. **Senha do SQL igual no Compose e na connection string** (já vem assim no repositório; veja [Configuração](#configuração)).

Não é obrigatório:

- ferramenta `dotnet-ef` na linha de comando — o `dotnet run` aplica as migrations sozinho;
- HTTPS de desenvolvimento — a API local é só HTTP em `http://localhost:5080` (perfil `http`). Não use o perfil `https` do Visual Studio / Rider;
- criar arquivo `.env` no frontend — o repositório já tem `frontend/.env.development` com `VITE_API_URL=http://localhost:5080`. Se a variável não existir, a interface usa o mesmo endereço.

## Conferência rápida do ambiente

Na **raiz do repositório** (a pasta que contém `docker-compose.yml` e as pastas `backend/` e `frontend/`):

```powershell
dotnet --version    # deve começar com 10.
node -v             # v20 ou maior
docker version      # Engine precisa responder; se falhar, abra o Docker Desktop e espere ficar verde
```

No PowerShell desta máquina, encadeie comandos com `;` (não use `&&`).

## Layout

```
backend/                 solution .NET (API + testes)
  src/Api/               um único .csproj, camadas em pastas
  tests/Api.Tests/
  Opus127.Dengue.sln
frontend/                React + TypeScript + Tailwind (Vite)
docs/                    PRD, ADRs e plano
docker-compose.yml       só o SQL Server
```

## Configuração

Os valores de desenvolvimento já estão no Git. Só mude se for sair do padrão.

### SQL Server (Compose + API)

`docker-compose.yml` sobe `mcr.microsoft.com/mssql/server:2022-latest` com:

- usuário `sa`
- senha **`Opus127_Dev!`** (somente desenvolvimento)
- porta do host `1433`

A API lê a connection string `Dengue` em `backend/src/Api/appsettings.json`:

```
Server=localhost,1433;Database=DengueAlerts;User Id=sa;Password=Opus127_Dev!;TrustServerCertificate=True
```

A senha **precisa ser a mesma** do `MSSQL_SA_PASSWORD` no Compose. Se alterar uma, altere a outra.

O banco `DengueAlerts` é criado na primeira subida da API (`Database.Migrate()` em `Program.cs`). Se o SQL ainda não estiver saudável, a API **não sobe** e o log pede para conferir o Docker e a connection string.

### Origem Infodengue

Em `appsettings.json`, seção `AlertaDengue`:

| Chave | Padrão | Significado |
|---|---|---|
| `BaseUrl` | `https://info.dengue.mat.br/` | origem dos boletins |
| `Geocode` | `3106200` | Belo Horizonte (fixo neste desafio) |

A carga sempre pede `disease=dengue` e `format=json`. Cidade e recorte de seis meses **não** são configuráveis pela interface.

### CORS e frontend

| Onde | Chave | Padrão |
|---|---|---|
| `appsettings.json` → `Cors:FrontendOrigin` | origem liberada | `http://localhost:5173` |
| `frontend/.env.development` | `VITE_API_URL` | `http://localhost:5080` |

O Vite está com `strictPort: true` na `5173`. Se a porta estiver ocupada, o `npm run dev` **falha** em vez de abrir `5174` (o que quebraria o CORS). Encerre o outro processo ou mude a porta **e** `Cors:FrontendOrigin` juntos.

Para apontar a interface para outra API, edite `VITE_API_URL` e **reinicie** o `npm run dev` (variável Vite só entra no boot).

Fuso usado no serviço e na tela: **America/Sao_Paulo**.

## Como executar (nessa ordem)

Três terminais. Sempre a partir da **raiz do repositório**, salvo o passo 3.

### 1. Banco

Na primeira vez o Docker baixa a imagem do SQL Server (pode levar vários minutos). Depois:

```powershell
docker compose up -d
```

Se o comando `docker compose` não existir, tente `docker-compose up -d`.

Espere o healthcheck (cerca de 20–40 s **depois** da imagem já estar baixada). Confira:

```powershell
docker compose ps
```

A coluna `STATUS` do serviço `sqlserver` precisa ter `healthy` (ou `running (healthy)`). Se estiver só `starting`, espere e rode `ps` de novo. **Não** suba a API antes disso.

### 2. API

```powershell
dotnet run --project backend/src/Api --launch-profile http
```

- URL: [http://localhost:5080](http://localhost:5080)
- Swagger (ambiente Development): [http://localhost:5080/swagger](http://localhost:5080/swagger)

Na inicialização a API:

1. aplica as migrations;
2. abre a porta **sem esperar** a Infodengue;
3. dispara a carga dos últimos seis meses em segundo plano (erros vão para o log; o processo continua).

A sync inicial pode levar alguns segundos. Enquanto ela não termina, um `GET` de semana recente pode voltar `404`. Se o log mostrar `Falha HTTP ao sincronizar alertas da AlertaDengue`, chame o `POST` da seção [Sincronização](#sincronização) com a rede ok.

### 3. Interface

Em **outro** terminal:

```powershell
cd frontend
npm install
npm run dev
```

Abre [http://localhost:5173](http://localhost:5173).

A tela calcula as três SEs fechadas, chama `GET /api/dengue?ew=&ey=` três vezes em paralelo e monta cartões, tabela e gráfico. `404` vira “Sem registro”. Falha de rede não mostra dados parciais.

### Como parar

- API e Vite: `Ctrl+C` em cada terminal.
- Banco (opcional; apaga só o container, não o código): `docker compose down` na raiz.

## Sincronização

Há **uma carga por vez** (trava em memória). Startup e `POST` não rodam em paralelo.

| Gatilho | Comportamento |
|---|---|
| Startup da API | automático, em background |
| `POST /api/dengue/sync` | reprocessa a janela de 6 meses sem reiniciar |

Se a janela cruzar o ano civil, a API faz **duas** chamadas à AlertaDengue (fim do ano anterior + início do atual). Gravação é upsert por par (ano, semana): reprocessar não duplica linha.

Exemplo de reprocessamento:

```powershell
Invoke-RestMethod -Method POST -Uri http://localhost:5080/api/dengue/sync
```

Resposta `200`: `{ "upserted": <quantidade de semanas gravadas> }`. Se a Infodengue falhar: `502`.

## Consulta (`GET`)

```
GET /api/dengue?ew={1-53}&ey={ano}
```

- `ew`: semana epidemiológica (1–53)
- `ey`: ano epidemiológico (2000–2100)
- Fora desses intervalos: `400`
- Semana inexistente no banco: `404`

A carga local **só** guarda a janela de ~6 meses. O JSON `2023-40` do enunciado é o **formato do contrato**, não uma semana que exista no seu banco depois do primeiro `run`. `GET ?ew=40&ey=2023` vai dar **404** — isso é esperado.

Depois da sync, use uma semana que o painel mostrar (rótulo `AAAA-SS` → `ey=AAAA` e `ew=SS`). Exemplo no PowerShell, ajustando semana e ano aos cartões da tela:

```powershell
Invoke-RestMethod "http://localhost:5080/api/dengue?ew=36&ey=2026"
```

`200` no contrato (números variam; o formato é este):

```json
{
  "semana_epidemiologica": "2026-36",
  "casos_est": 722,
  "casos_notificados": 37,
  "nivel_alerta": 3
}
```

Na origem, `SE` é inteiro `AAAASS` (ex.: `202340`). O banco guarda ano e semana; a API devolve `"AAAA-SS"`.

## Testes

**Pare a API** (`Ctrl+C`) antes de testar no Windows: com o processo rodando, o `dotnet test` pode falhar ao copiar o `exe`/`dll` em `bin/Debug`. Docker Desktop precisa estar aberto.

```powershell
dotnet test backend/Opus127.Dengue.sln
```

- Isolados: calendário epidemiológico, janela de 6 meses, mapeamento `SE` → contrato, sync com origem falsa.
- Integração: `WebApplicationFactory` + **SQL Server real** via Testcontainers (Docker obrigatório).

CI no GitHub Actions (`.github/workflows/ci.yml`): o mesmo `dotnet test` em `main` e PRs. Sem deploy.

A interface **não** tem bateria automatizada; a verificação é o checklist abaixo.

## Deu certo? Checklist de fumaça

Faça nesta ordem. Se um passo falhar, pare e use [Problemas comuns](#problemas-comuns).

1. `docker compose ps` → `sqlserver` **healthy**.
2. `http://localhost:5080/swagger` abre e lista `GET /api/dengue` e `POST /api/dengue/sync`.
3. `http://localhost:5173` mostra três cartões (ou “Sem registro” se a Infodengue ainda não publicou aquela SE).
4. O rótulo de um cartão preenchido, usado no `GET ?ew=&ey=`, devolve `200` com as quatro chaves do contrato.

## Problemas comuns

| Sintoma | O que conferir |
|---|---|
| `docker version` falha / `compose` não acha o daemon | Docker Desktop aberto e “engine running”; espere o ícone ficar parado |
| `compose` baixa sem parar / demora | primeira vez baixa ~1 GB; não é erro |
| API não sobe / erro de migrate | `docker compose ps` **healthy**; senha `Opus127_Dev!` igual no Compose e no `appsettings.json`; porta `1433` livre (pare o SQL Server local do Windows se estiver instalado) |
| `GET` de `2023-40` dá 404 | esperado: essa semana não entra na janela de 6 meses. Use uma semana do painel |
| `GET` recente sempre 404 | sync ainda não terminou ou a Infodengue falhou; veja o log (`Falha HTTP ao sincronizar…`) e chame `POST /api/dengue/sync` |
| Frontend em branco / CORS no DevTools | API em `5080`, Vite **exato** em `5173`, `VITE_API_URL` e `Cors:FrontendOrigin` alinhados; reinicie o Vite depois de editar `.env` |
| `Port 5173 is already in use` | outro Vite/processo na porta; encerre-o. O projeto não muda sozinho para `5174` |
| `dotnet test` falha com arquivo bloqueado | encerre `Opus127.Dengue.Api` e rode de novo |
| Testes de integração estouram timeout | Docker Desktop precisa estar aberto |

## Documentação

- [Documento de requisitos](docs/prd.md)
- [Registros de decisão](docs/adr/)
- [Plano de implementação](docs/plans/2026-09-16-desafio-opus127.md)
- [README da interface](frontend/README.md)

Git: `main` estável e uma branch por fatia (`feat/...`).
