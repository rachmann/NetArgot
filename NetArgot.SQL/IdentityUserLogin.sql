CREATE TABLE [dbo].[IdentityUserLogin]
(
	[UserId] INT NOT NULL,
    [LoginProvider] NVARCHAR(200) NOT NULL, 
    [ProviderKey] NVARCHAR(200) NOT NULL,
	CONSTRAINT [PK_dbo.IdentityUserLogin] PRIMARY KEY CLUSTERED ([UserId] ASC)
)
GO
CREATE UNIQUE NONCLUSTERED INDEX [LoginProviderIndex] ON [dbo].[IdentityUserLogin]([LoginProvider] ASC);