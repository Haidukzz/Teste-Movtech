-- ============================================================
-- Script SQL - Order Management API
-- Banco: PostgreSQL
-- Gerado para: PedidoClientManagement
-- ============================================================

-- Tabela de Clientes
CREATE TABLE IF NOT EXISTS "Clientes" (
    "Id"             SERIAL PRIMARY KEY,
    "Nome"           TEXT        NOT NULL,
    "Email"          TEXT        NOT NULL,
    "DataNascimento" TIMESTAMPTZ NOT NULL,
    "CPF"            VARCHAR(11) NOT NULL,
    "Endereco"       TEXT        NOT NULL
);

-- Índice único para garantir CPF sem duplicatas
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Clientes_CPF"
    ON "Clientes" ("CPF");

-- Tabela de Pedidos
CREATE TABLE IF NOT EXISTS "Pedidos" (
    "Id"         SERIAL PRIMARY KEY,
    "ClienteId"  INTEGER     NOT NULL,
    "DataPedido" TIMESTAMPTZ NOT NULL,
    "ValorTotal" NUMERIC(18,2) NOT NULL,
    CONSTRAINT "FK_Pedidos_Clientes_ClienteId"
        FOREIGN KEY ("ClienteId")
        REFERENCES "Clientes" ("Id")
        ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_Pedidos_ClienteId"
    ON "Pedidos" ("ClienteId");

-- Tabela de Itens do Pedido
CREATE TABLE IF NOT EXISTS "ItensPedido" (
    "Id"            SERIAL PRIMARY KEY,
    "PedidoId"      INTEGER      NOT NULL,
    "Descricao"     TEXT         NOT NULL,
    "Quantidade"    INTEGER      NOT NULL,
    "PrecoUnitario" NUMERIC(18,2) NOT NULL,
    CONSTRAINT "FK_ItensPedido_Pedidos_PedidoId"
        FOREIGN KEY ("PedidoId")
        REFERENCES "Pedidos" ("Id")
        ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_ItensPedido_PedidoId"
    ON "ItensPedido" ("PedidoId");

-- Tabela de controle de migrations do EF Core (gerada automaticamente, incluída para referência)
-- CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
--     "MigrationId"    VARCHAR(150) NOT NULL PRIMARY KEY,
--     "ProductVersion" VARCHAR(32)  NOT NULL
-- );
