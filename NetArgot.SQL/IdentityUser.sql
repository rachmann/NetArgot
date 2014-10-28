CREATE TABLE [dbo].[IdentityUser]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [Description] NVARCHAR(200) NULL, 
    [UserName] NVARCHAR(50) NOT NULL, 
    [Email] NVARCHAR(200) NOT NULL, 
    [EmailConfirmed] BIT NOT NULL, 
    [PasswordHash] NVARCHAR(200) NOT NULL, 
    [SecurityStamp] NVARCHAR(200) NOT NULL, 
    [PhoneNumber] NVARCHAR(20) NOT NULL, 
    [PhoneNumberConfirmed] BIT NOT NULL, 
    [TwoFactorEnabled] BIT NOT NULL, 
    [LockoutEndDateUtc] DATETIME NULL, 
    [LockoutEnabled] BIT NOT NULL, 
    [AccessFailedCount] INT NOT NULL, 
    [ApplicationName] NVARCHAR(50) NULL,
	CONSTRAINT [PK_dbo.IdentityUser] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[IdentityUser]([UserName] ASC);