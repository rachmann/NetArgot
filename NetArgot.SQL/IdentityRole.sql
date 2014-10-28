CREATE TABLE [dbo].[IdentityRole]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(50) NULL,
	CONSTRAINT [PK_dbo.IdentityRole] PRIMARY KEY CLUSTERED ([Id] ASC)
)

GO

CREATE INDEX [IX_RoleNameIndex] ON [dbo].[IdentityRole] ([Name])
