CREATE TABLE [dbo].[IdentityUserClaimType]
(
	[TypeId] INT IDENTITY (1, 1) NOT NULL,  
    [ClaimTypeCode] NVARCHAR(50) NOT NULL, 
    [ClaimTypeDescription] NVARCHAR(200) NOT NULL,
	CONSTRAINT [PK_dbo.IdentityUserClaimType] PRIMARY KEY CLUSTERED ([TypeId] ASC)
)
