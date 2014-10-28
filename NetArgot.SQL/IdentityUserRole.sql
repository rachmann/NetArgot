CREATE TABLE [dbo].[IdentityUserRole]
(
	[UserId] INT NOT NULL, 
    [RoleId] INT NOT NULL,
	CONSTRAINT [PK_dbo.IdentityUserRole] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_dbo.IdentityUserRole_dbo.IdentityUserRole_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[IdentityRole] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.IdentityUserRole_dbo.IdentityUserRole_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[IdentityUser] ([Id]) ON DELETE CASCADE
)
