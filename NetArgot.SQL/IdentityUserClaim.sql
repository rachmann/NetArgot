CREATE TABLE [dbo].[IdentityUserClaim]
(
	[ClaimId] INT IDENTITY (1, 1) NOT NULL,
    [UserId] INT NOT NULL, 
    [ClaimTypeId] INT NOT NULL, 
    [ClaimValue] NVARCHAR(20) NOT NULL, 
    [ClaimValueType] NVARCHAR(20) NOT NULL, 
    [Issuer] NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_dbo.IdentityUserClaim] PRIMARY KEY CLUSTERED ([ClaimId] ASC),
    CONSTRAINT [FK_dbo.IdentityUserClaim_dbo.IdentityUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[IdentityUser] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[IdentityUserClaim]([UserId] ASC);
