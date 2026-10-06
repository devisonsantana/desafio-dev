![CI](https://github.com/devisonsantana/desafio-dev/actions/workflows/ci.yml/badge.svg)

# Desafio técnico

Aplicação de console em .NET 10 para resolver três exercícios: cálculo de comissão por vendedor, movimentação de estoque e cálculo de juros simples por atraso.

## Estrutura

- `src/Desafio.Console`: menu, interação com o usuário e arquivos JSON usados como dados iniciais.
- `src/Desafio.Core`: modelos de domínio (`Sale`, `Product` e modelos dos arquivos JSON), com validações básicas.
- `src/Desafio.Infrastructure`: leitura de JSON e regras de cálculo de comissão, estoque e juros.
- `tests/Desafio.Core.Tests` e `tests/Desafio.Infrastructure.Tests`: testes automatizados com xUnit.

A aplicação usa APIs do .NET para JSON e não depende de pacotes externos em tempo de execução. Os projetos de teste usam xUnit e Coverlet.

## Requisitos e comandos

É necessário ter o SDK do .NET 10 instalado. Na raiz da solução:

```bash
dotnet build
dotnet test
dotnet run --project src/Desafio.Console
```

O workflow em `.github/workflows/ci.yml` restaura, compila e executa os testes em pushes e pull requests para `main`.

## Funcionalidades

### 1. Comissão por vendedor

As vendas de exemplo são lidas de [`src/Desafio.Console/Data/vendas.json`](src/Desafio.Console/Data/vendas.json). A regra é aplicada a cada venda:

- Menos de R$ 100,00: sem comissão.
- De R$ 100,00 (inclusive) a menos de R$ 500,00: 1%.
- R$ 500,00 ou mais: 5%.

Os valores são calculados usando `decimal`. A comissão de cada venda não é arredondada antes da soma; o console formata os valores monetários com duas casas decimais. Nomes com espaços externos são normalizados, mas diferenças entre maiúsculas e minúsculas ainda formam grupos distintos. Valores negativos e nomes vazios são rejeitados.

Resultado exibido para o arquivo de exemplo:

| Vendedor        | Total vendido |  Comissão |
| --------------- | ------------: | --------: |
| Ana Lima        |   R$ 8.763,95 | R$ 404,98 |
| Carlos Oliveira |   R$ 7.928,35 | R$ 379,37 |
| João Silva      |  R$ 10.754,70 | R$ 495,68 |
| Maria Souza     |   R$ 9.874,30 | R$ 465,95 |

### 2. Movimentação de estoque

O estoque inicial é lido de [`src/Desafio.Console/Data/estoque.json`](src/Desafio.Console/Data/estoque.json) quando a opção de estoque é aberta pela primeira vez. É possível consultar os produtos, registrar entradas e saídas e consultar o histórico durante a execução.

Cada movimentação recebe um identificador sequencial iniciado em 1, data/hora local, código do produto, tipo, descrição, quantidade e saldo após a operação. A quantidade precisa ser positiva, a descrição não pode estar vazia, o produto precisa existir e uma saída não pode exceder o saldo. Os saldos e o histórico ficam apenas em memória: retornam ao estoque inicial ao encerrar e abrir novamente o programa.

### 3. Juros por atraso

Informe um valor não negativo e uma data de vencimento no formato `dd/MM/aaaa`. O cálculo usa juros simples de 2,5% ao dia corrido:

```text
juros = valor × 0,025 × dias em atraso
```

No dia do vencimento ou antes dele, os juros são zero. O valor de juros é arredondado para duas casas decimais com `MidpointRounding.AwayFromZero`; o total exibido soma o valor informado e os juros.

Exemplo: R$ 100,00 com vencimento em 10/03/2025, calculado em 15/03/2025, resulta em 5 dias de atraso, R$ 12,50 de juros e R$ 112,50 no total.

## Observações

- Os arquivos JSON são copiados para a pasta `Data` junto à saída do projeto Console.
- O estoque não é persistido em arquivo; as alterações existem somente até o encerramento do processo.
- O cálculo de juros recebe as datas como `DateOnly`; a aplicação obtém a data atual do relógio local do sistema.
