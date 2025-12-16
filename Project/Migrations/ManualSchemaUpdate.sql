-- Add new columns to Repairs table
ALTER TABLE [Repairs] ADD [LaborHours] decimal(18,2) NOT NULL DEFAULT 0;
ALTER TABLE [Repairs] ADD [LaborCost] decimal(18,2) NOT NULL DEFAULT 0;
ALTER TABLE [Repairs] ADD [PartsCost] decimal(18,2) NOT NULL DEFAULT 0;
ALTER TABLE [Repairs] ADD [TotalCost] decimal(18,2) NOT NULL DEFAULT 0;
ALTER TABLE [Repairs] ADD [InvoiceNumber] nvarchar(50) NULL;
ALTER TABLE [Repairs] ADD [InvoiceGeneratedOn] datetime2 NULL;

-- Create PriceSettings table
CREATE TABLE [PriceSettings] (
    [Id] uniqueidentifier NOT NULL,
    [LaborCostPerHour] decimal(18,2) NOT NULL,
    [PartsMarkupPercent] decimal(5,2) NOT NULL,
    [VATPercent] decimal(5,2) NOT NULL,
    [DiagnosticFee] decimal(18,2) NOT NULL,
    [CompanyName] nvarchar(100) NULL,
    [CompanyAddress] nvarchar(500) NULL,
    [CompanyPhone] nvarchar(50) NULL,
    [CompanyEmail] nvarchar(100) NULL,
    [CompanyVATNumber] nvarchar(50) NULL,
    [CompanyRegistrationNumber] nvarchar(50) NULL,
    [IsActive] bit NOT NULL,
    [CreatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_PriceSettings] PRIMARY KEY ([Id])
);

-- Insert default settings
INSERT INTO [PriceSettings] ([Id], [LaborCostPerHour], [PartsMarkupPercent], [VATPercent], [DiagnosticFee], [CompanyName], [CompanyAddress], [CompanyPhone], [CompanyEmail], [IsActive], [CreatedOn])
VALUES ('11111111-1111-1111-1111-111111111111', 50.00, 20.00, 20.00, 30.00, 'BidMotors', N'София, България', '+359 888 123 456', 'office@bidmotors.bg', 1, GETUTCDATE());

GO
