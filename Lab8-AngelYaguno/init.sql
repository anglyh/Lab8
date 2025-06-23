-- Crear tablas si no existen (PostgreSQL)
CREATE TABLE IF NOT EXISTS clients (
    "ClientId" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Email" VARCHAR(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS products (
    "ProductId" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Description" VARCHAR(100),
    "Price" DECIMAL(10,2) NOT NULL
);

CREATE TABLE IF NOT EXISTS orders (
    "OrderId" SERIAL PRIMARY KEY,
    "ClientId" INTEGER NOT NULL,
    "OrderDate" TIMESTAMP NOT NULL,
    CONSTRAINT "FK_orders_clients" FOREIGN KEY ("ClientId") REFERENCES clients("ClientId")
);

CREATE TABLE IF NOT EXISTS orderdetails (
    "OrderDetailId" SERIAL PRIMARY KEY,
    "OrderId" INTEGER NOT NULL,
    "ProductId" INTEGER NOT NULL,
    "Quantity" INTEGER NOT NULL,
    CONSTRAINT "FK_orderdetails_orders" FOREIGN KEY ("OrderId") REFERENCES orders("OrderId"),
    CONSTRAINT "FK_orderdetails_products" FOREIGN KEY ("ProductId") REFERENCES products("ProductId")
);

-- Insertar datos de ejemplo
INSERT INTO clients ("Name", "Email") VALUES 
('Juan Pérez', 'juan.perez@email.com'),
('Maria González', 'maria.gonzalez@email.com'),
('Carlos Rodríguez', 'carlos.rodriguez@email.com'),
('Ana López', 'ana.lopez@email.com')
ON CONFLICT DO NOTHING;

INSERT INTO products ("Name", "Description", "Price") VALUES 
('Laptop Dell', 'Laptop de alta gama', 1200.00),
('Mouse Logitech', 'Mouse inalámbrico', 35.50),
('Teclado Mecánico', 'Teclado gaming RGB', 89.99),
('Monitor 24"', 'Monitor Full HD', 299.99),
('Auriculares Sony', 'Auriculares con cancelación de ruido', 150.00),
('Webcam HD', NULL, 45.99)
ON CONFLICT DO NOTHING;

INSERT INTO orders ("ClientId", "OrderDate") VALUES 
(1, '2024-01-15 10:30:00'),
(2, '2024-01-16 14:20:00'),
(1, '2024-01-17 09:45:00'),
(3, '2024-01-18 16:10:00'),
(4, '2024-01-19 11:30:00')
ON CONFLICT DO NOTHING;

INSERT INTO orderdetails ("OrderId", "ProductId", "Quantity") VALUES 
(1, 1, 1),
(1, 2, 2),
(2, 3, 1),
(2, 4, 1),
(3, 5, 1),
(4, 1, 1),
(4, 6, 3),
(5, 2, 1),
(5, 3, 1)
ON CONFLICT DO NOTHING; 