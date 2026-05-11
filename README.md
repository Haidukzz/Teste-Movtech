# Order Management API

## Descrição do Projeto

API web para gerenciamento de **clientes** e **pedidos**, desenvolvida em C# (.NET 8) com Entity Framework Core e PostgreSQL.  
Expõe endpoints RESTful para CRUD de clientes e pedidos, com interface web (HTML/JS) integrada.

## Tecnologias

- **ASP.NET Core (.NET 8)** — framework principal
- **Entity Framework Core** — ORM com Migrations code-first
- **PostgreSQL** — banco de dados relacional (hospedado no Render)
- **Render** — plataforma de deploy (API + banco)

## Funcionalidades

### Clientes
- Cadastro com: Nome, Email, CPF, Data de Nascimento e **Endereço**
- Validações: CPF único e válido, e-mail com formato correto, idade ≥ 18 anos
- Login por CPF + Data de Nascimento
- Edição e exclusão de clientes
- Consulta de **total gasto** por cliente

### Pedidos
- Criação com ao menos um item (Descrição, Quantidade, Preço Unitário)
- Valor total calculado automaticamente a partir dos itens
- Bloqueio de alteração após 24 horas da criação
- Filtragem por nome do cliente e/ou intervalo de datas
- Exibição de detalhes com itens e total gasto

## Estrutura do Projeto

```
PedidoClientManagement.API/
├── Controllers/
│   ├── ClientesController.cs    # GET, POST, PUT, DELETE /api/clientes
│   └── PedidosController.cs     # GET, POST, PUT, DELETE /api/pedidos
├── Models/
│   ├── Cliente.cs               # Nome, Email, CPF, DataNascimento, Endereco
│   ├── Pedido.cs                # ClienteId, DataPedido, ValorTotal, Itens
│   └── ItemPedido.cs            # Descricao, Quantidade, PrecoUnitario
├── Data/
│   ├── AppDbContext.cs
│   └── Migrations/
│       ├── 20250512181415_InicialPostgres.cs
│       └── 20250508120000_AddEnderecoCliente.cs
├── wwwroot/
│   ├── index.html               # Cadastro de cliente
│   ├── login.html               # Login de cliente
│   ├── pedidos.html             # Criação de pedido
│   ├── pedidos-list.html        # Listagem e filtro de pedidos
│   ├── pedidos-detail.html      # Detalhes de pedido
│   └── clientes-list.html       # Gerenciamento de clientes
└── Program.cs
schema.sql                       # Script SQL para criação das tabelas
```

## Endpoints Principais

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | /api/clientes | Listar todos os clientes |
| GET | /api/clientes/{id} | Buscar cliente por ID |
| POST | /api/clientes | Cadastrar cliente |
| PUT | /api/clientes/{id} | Editar cliente |
| DELETE | /api/clientes/{id} | Excluir cliente |
| GET | /api/clientes/{id}/total-gasto | Total gasto pelo cliente |
| GET | /api/clientes/auth?cpf=&dataNascimento= | Login |
| GET | /api/pedidos | Listar todos os pedidos |
| GET | /api/pedidos/{id} | Buscar pedido por ID |
| POST | /api/pedidos | Criar pedido |
| PUT | /api/pedidos/{id} | Atualizar pedido (dentro de 24h) |
| DELETE | /api/pedidos/{id} | Excluir pedido |
| GET | /api/pedidos/filtro?nome=&inicio=&fim= | Filtrar pedidos |

## Configuração

A connection string é lida da variável de ambiente `DATABASE_URL` (ou `ConnectionStrings:DefaultConnection` no appsettings).  
No Render, configure a variável de ambiente `DATABASE_URL` com a connection string PostgreSQL.

Exemplo para desenvolvimento local (`appsettings.Development.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=pedidos_db;Username=postgres;Password=sua_senha"
  }
}
```

## Script SQL

O arquivo `schema.sql` na raiz do repositório contém o script completo para criação das tabelas no PostgreSQL.

Link da aplicação: 
https://pedido-client-management.onrender.com
