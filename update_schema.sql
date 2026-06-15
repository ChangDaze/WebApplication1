BEGIN TRANSACTION;
CREATE TABLE [CartItems] (
    [Id] int NOT NULL IDENTITY,
    [SessionId] nvarchar(max) NOT NULL,
    [ProductId] int NOT NULL,
    [Quantity] int NOT NULL,
    [DateCreated] datetime2 NOT NULL,
    CONSTRAINT [PK_CartItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CartItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Orders] (
    [Id] int NOT NULL IDENTITY,
    [CustomerName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [ShippingAddress] nvarchar(max) NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [OrderDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id])
);

CREATE TABLE [OrderDetails] (
    [Id] int NOT NULL IDENTITY,
    [OrderId] int NOT NULL,
    [ProductId] int NOT NULL,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_OrderDetails] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderDetails_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderDetails_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_CartItems_ProductId] ON [CartItems] ([ProductId]);

CREATE INDEX [IX_OrderDetails_OrderId] ON [OrderDetails] ([OrderId]);

CREATE INDEX [IX_OrderDetails_ProductId] ON [OrderDetails] ([ProductId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260615210315_AddShoppingCart', N'10.0.9');

COMMIT;
GO

