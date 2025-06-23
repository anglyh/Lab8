-- Script de inicialización para PostgreSQL
-- Adaptado para funcionar con la base de datos lab14_db en Render

-- Nota: En Render no podemos crear la base de datos, ya existe como lab14_db
-- \c lab14_db;

-- Crear tabla de clientes
CREATE TABLE IF NOT EXISTS "Clients" (
    "ClientId" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Email" VARCHAR(100) NOT NULL
);

-- Crear tabla de productos
CREATE TABLE IF NOT EXISTS "Products" (
    "ProductId" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Description" VARCHAR(100) NULL,
    "Price" DECIMAL(10, 2) NOT NULL
);

-- Crear tabla de pedidos (Orders)
CREATE TABLE IF NOT EXISTS "Orders" (
    "OrderId" SERIAL PRIMARY KEY,
    "ClientId" INTEGER NOT NULL,
    "OrderDate" TIMESTAMP NOT NULL,
    FOREIGN KEY ("ClientId") REFERENCES "Clients"("ClientId")
);

-- Crear tabla de detalles de la orden (OrderDetails)
CREATE TABLE IF NOT EXISTS "OrderDetails" (
    "OrderDetailId" SERIAL PRIMARY KEY,
    "OrderId" INTEGER NOT NULL,
    "ProductId" INTEGER NOT NULL,
    "Quantity" INTEGER NOT NULL,
    FOREIGN KEY ("OrderId") REFERENCES "Orders"("OrderId"),
    FOREIGN KEY ("ProductId") REFERENCES "Products"("ProductId")
);

-- Limpiar datos existentes (si los hay)
TRUNCATE TABLE "OrderDetails", "Orders", "Products", "Clients" RESTART IDENTITY CASCADE;

-- Insertar clientes
INSERT INTO "Clients" ("Name", "Email") VALUES
('Juan Pérez', 'juan.perez@example.com'),
('Ana Gómez', 'ana.gomez@example.com'),
('Carlos Díaz', 'carlos.diaz@example.com'),
('Lucía Martínez', 'lucia.martinez@example.com');

-- Insertar productos
INSERT INTO "Products" ("Name", "Price", "Description") VALUES
('Producto A', 10.50, 'This product is cheap'),
('Producto B', 25.00, 'This product is pretty'),
('Producto C', 15.75, 'This product is awesome'),
('Producto D', 30.20, NULL);

-- Insertar pedidos
INSERT INTO "Orders" ("ClientId", "OrderDate") VALUES
(1, '2025-05-01 10:00:00'),
(1, '2025-05-02 11:00:00'),
(2, '2025-05-03 12:00:00'),
(2, '2025-05-04 13:00:00'),
(3, '2025-05-05 14:00:00'),
(3, '2025-05-06 15:00:00'),
(4, '2025-05-07 16:00:00');

-- Insertar detalles de la orden (OrderDetails)
INSERT INTO "OrderDetails" ("OrderId", "ProductId", "Quantity") VALUES
(1, 2, 2), -- Orden 1: 2 Productos B
(1, 1, 1), -- Orden 1: 1 Producto A
(2, 3, 1), -- Orden 2: 1 Producto C
(2, 4, 1), -- Orden 2: 1 Producto D
(3, 1, 3), -- Orden 3: 3 Productos A
(3, 2, 2), -- Orden 3: 2 Productos B
(4, 3, 1); -- Orden 4: 1 Producto C

-- Verificar los datos insertados
SELECT 'Clients' as tabla, COUNT(*) as registros FROM "Clients"
UNION ALL
SELECT 'Products' as tabla, COUNT(*) as registros FROM "Products"
UNION ALL
SELECT 'Orders' as tabla, COUNT(*) as registros FROM "Orders"
UNION ALL
SELECT 'OrderDetails' as tabla, COUNT(*) as registros FROM "OrderDetails"; 