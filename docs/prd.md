# Documento de requisitos — Alertas de dengue em Belo Horizonte

## Problema

Equipes de saúde e gestão precisam acompanhar o alerta de dengue em Belo Horizonte com números oficiais da Infodengue, sem consultar a origem a cada tela. Falta um serviço próprio que traga as últimas 26 semanas, guarde no banco da organização e permita consultar uma semana epidemiológica e ver as três semanas mais recentes numa interface clara.

## Solução

Um serviço em .NET que busca os boletins de Belo Horizonte (código IBGE `3106200`) na AlertaDengue, grava no SQL Server e responde consultas por ano e semana. Uma aplicação web em React mostra as três últimas semanas, cada uma obtida por uma consulta própria ao serviço. O banco sobe em contêiner; o serviço e a interface rodam na máquina de quem opera. Requisitos e decisões ficam neste documento e nos registros de arquitetura.

## Histórias de usuário

1. Como operador, quero um guia de execução que suba o banco, o serviço e a interface, para não depender de configuração informal.
2. Como operador, quero que o serviço busque dengue de Belo Horizonte nos últimos seis meses na AlertaDengue, para a base refletir o período recente.
3. Como operador, quero esses dados no SQL Server, para consultas internas não dependerem da Infodengue a cada pedido.
4. Como operador, quero reprocessar a carga com um pedido `POST /api/dengue/sync`, para atualizar o banco sem reiniciar o processo.
5. Como operador, quero que a carga rode sozinha na inicialização, para a primeira subida já trazer dados.
6. Como operador, quero que uma falha da AlertaDengue não derrube o serviço, para eu ainda ler o que já estiver gravado.
7. Como consumidor do serviço, quero consultar com `ew` (semana) e `ey` (ano) e receber o JSON abaixo, para integrar telas e relatórios.
8. Como consumidor do serviço, quero resposta 404 quando a semana não existir, para tratar ausência de boletim de forma previsível.
9. Como aplicação web, quero `semana_epidemiologica` no formato `"2023-40"`, para exibir o rótulo de semana de forma estável.
10. Como aplicação web, quero três consultas de leitura (uma por semana), para montar o painel sem um ponto agregado no servidor.
11. Como cidadão ou gestor, quero três cartões com casos estimados, notificados e nível de alerta, para ler o recorte rápido.
12. Como cidadão ou gestor, quero uma tabela com as mesmas colunas, para comparar as três semanas.
13. Como cidadão ou gestor, quero um gráfico de estimados versus notificados, para ver a tendência.
14. Como cidadão ou gestor, quero cores de alerta (1 a 4) nos cartões, para o nível ser imediato.
15. Como cidadão ou gestor, quero carregamento e estado vazio por semana, para a tela não quebrar se faltar boletim.
16. Como desenvolvedor, quero gravar por par (ano, semana) atualizando o que já existe, para reprocessar sem duplicar.
17. Como desenvolvedor, quero duas chamadas à origem se a janela de seis meses cruzar o ano, para respeitar os parâmetros de semana da AlertaDengue.
18. Como desenvolvedor, quero testes isolados do calendário epidemiológico e da conversão de `SE` para o contrato, para não depender da rede.
19. Como desenvolvedor, quero testes de ponta a ponta do serviço contra SQL Server de verdade, para a gravação e a leitura serem as da operação.
20. Como desenvolvedor, quero registros de decisão de arquitetura, para o raciocínio ficar junto do código.

Contrato de leitura (`GET /api/dengue?ew={1-53}&ey={ano}`):

```json
{
  "semana_epidemiologica": "2023-40",
  "casos_est": 45,
  "casos_notificados": 38,
  "nivel_alerta": 2
}
```

## Decisões de implementação

- Repositório em duas raízes de aplicação: `backend/` (solution .NET, código em `backend/src`, testes em `backend/tests`) e `frontend/` (React, TypeScript, Tailwind, código em `frontend/src`). Documentação em `docs/`. Banco no `docker-compose.yml` da raiz.
- Um único projeto de API (`backend/src/Api`), com pastas de controle, regra de negócio e persistência. Sem projetos separados de Domínio e Infraestrutura neste momento: as camadas existem como pastas, não como vários `.csproj`.
- Sem mediadores extras e sem outro framework web além do React.
- SQL Server apenas no `docker-compose.yml`. Serviço e interface na máquina de quem executa.
- Persistência com Entity Framework Core. Índice único em ano e semana epidemiológica.
- Colunas gravadas: ano, semana (1 a 53), casos estimados, casos notificados, nível de alerta, código IBGE (`3106200`) e instante da sincronização. Sem guardar o dicionário completo da Infodengue.
- Origem: `https://info.dengue.mat.br/api/alertcity` com `geocode=3106200`, `disease=dengue`, `format=json` e intervalo de semanas e anos.
- Na origem, `SE` é inteiro `AAAASS` (exemplo: `202602`). O banco guarda ano e semana. A resposta do serviço usa `"AAAA-SS"` com semana em dois dígitos.
- Calendário Infodengue: semana começa no **domingo**. A semana 1 do ano é a que contém 4 de janeiro. “Hoje” no serviço e na interface usa o fuso **America/Sao_Paulo**. Os testes fixam um `SE` conhecido da origem.
- Janela de carga: agora em América/São Paulo menos seis meses civis até agora; converter os extremos em ano e semana; se o ano mudar, duas buscas (fim do ano anterior e início do atual). A semana final do ano anterior pode ser 53.
- Carga: um serviço em segundo plano na inicialização (erros no registro de log; o processo segue) e o pedido `POST /api/dengue/sync`. No processo, **uma carga por vez** (trava em memória).
- OpenAPI/Swagger sempre disponível em `/swagger`.
- API local só em HTTP: `http://localhost:5080` (sem HTTPS de desenvolvimento).
- Nomes do JSON de leitura: `casos_notificados` e `nivel_alerta`, mesmo a origem usar `casos` e `nivel`.
- Origens cruzadas liberadas só para a interface em desenvolvimento (`http://localhost:5173`). Endereço do serviço na variável `VITE_API_URL`.
- A interface calcula as três últimas semanas (mesmo calendário) e faz três leituras em paralelo. Não há rota de “últimas três” no servidor.
- Uma tela: cartões, tabela e gráfico. Sem autenticação. Cidade fixa no serviço.
- A carga na inicialização não prende a abertura da porta do serviço: roda em segundo plano.

## Decisões de teste

- Validar comportamento visível (janela de datas, contrato, respostas 200 e 404, gravação sem duplicar), não detalhes internos do mapeamento objeto-relacional.
- Isolados (sem rede e sem banco): calendário e janela de seis meses; conversão `SE` ↔ contrato; serviço de carga com origem falsa — grava o recorte combinado, atualiza registro existente, e a falha da origem é tratada na inicialização sem derrubar o processo.
- Integrados: fábrica da aplicação web contra **SQL Server real** em contêiner de teste. Leitura 200 no JSON combinado; leitura 404; reprocessamento com origem falsa gravando no banco.
- Interface: verificação manual descrita no guia de execução; sem bateria automatizada de tela nesta versão.
- Ferramentas: xUnit, FluentAssertions, NSubstitute, Microsoft.AspNetCore.Mvc.Testing e Testcontainers.

## Fora de escopo

- Outras cidades, zika, chikungunya ou período configurável além de seis meses em Belo Horizonte.
- Autenticação, autorização, limite de tráfego e cache distribuído.
- Gravar o JSON completo da AlertaDengue.
- Listagem ou recorte “últimas três semanas” no servidor.
- Outro framework de interface, rotas mínimas como superfície principal, outro acesso a dados que não o Entity Framework, ou vários projetos de domínio.
- Testes automatizados da interface.
- Publicação em nuvem; a operação prevista é local, com guia de execução.
- Validação defensiva de linhas podres da Infodengue e nova tentativa automática na interface se a primeira leitura vier vazia (fica para depois).

## Notas adicionais

- Organização e registros de decisão fazem parte do produto, junto com o código.
- Risco: Infodengue lenta ou indisponível — mitigado por gravação local, leitura só no banco e inicialização que não encerra o processo.
- Risco: semana corrente ainda sem boletim — a interface mostra vazio naquele cartão.
- Risco: virada de ano e anos com 53 semanas — coberto pelos testes isolados da janela.
