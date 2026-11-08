IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [FullName] nvarchar(100) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE TABLE [N5Skills] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [NormalizedName] nvarchar(100) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_N5Skills] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_N5Skills_Name] CHECK (LEN(LTRIM(RTRIM([Name]))) > 0 AND LEN(LTRIM(RTRIM([NormalizedName]))) > 0)
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE TABLE [N5CandidateProfiles] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [Phone] nvarchar(30) NULL,
        [Location] nvarchar(200) NULL,
        [Introduction] nvarchar(2000) NOT NULL,
        [Education] nvarchar(4000) NULL,
        [Experience] nvarchar(4000) NULL,
        CONSTRAINT [PK_N5CandidateProfiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_N5CandidateProfiles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE TABLE [N5Companies] (
        [Id] int NOT NULL IDENTITY,
        [EmployerUserId] nvarchar(450) NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(4000) NULL,
        [Address] nvarchar(500) NULL,
        [Website] nvarchar(2048) NULL,
        CONSTRAINT [PK_N5Companies] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_N5Companies_AspNetUsers_EmployerUserId] FOREIGN KEY ([EmployerUserId]) REFERENCES [AspNetUsers] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE TABLE [N5CandidateSkills] (
        [CandidateProfileId] int NOT NULL,
        [SkillId] int NOT NULL,
        CONSTRAINT [PK_N5CandidateSkills] PRIMARY KEY ([CandidateProfileId], [SkillId]),
        CONSTRAINT [FK_N5CandidateSkills_N5CandidateProfiles_CandidateProfileId] FOREIGN KEY ([CandidateProfileId]) REFERENCES [N5CandidateProfiles] ([Id]),
        CONSTRAINT [FK_N5CandidateSkills_N5Skills_SkillId] FOREIGN KEY ([SkillId]) REFERENCES [N5Skills] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE TABLE [N5JobPostings] (
        [Id] int NOT NULL IDENTITY,
        [CompanyId] int NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [Location] nvarchar(200) NULL,
        [SalaryText] nvarchar(200) NULL,
        [PublishedAt] datetimeoffset NULL,
        [Deadline] datetimeoffset NOT NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_N5JobPostings] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_N5JobPostings_PublishedAt] CHECK ([Status] = 0 OR [PublishedAt] IS NOT NULL),
        CONSTRAINT [CK_N5JobPostings_Status] CHECK ([Status] IN (0, 1, 2)),
        CONSTRAINT [CK_N5JobPostings_Utc] CHECK (DATEPART(TZOFFSET, [Deadline]) = 0 AND ([PublishedAt] IS NULL OR DATEPART(TZOFFSET, [PublishedAt]) = 0)),
        CONSTRAINT [FK_N5JobPostings_N5Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [N5Companies] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE TABLE [N5Applications] (
        [Id] int NOT NULL IDENTITY,
        [JobPostingId] int NOT NULL,
        [CandidateProfileId] int NOT NULL,
        [CoverLetter] nvarchar(2000) NULL,
        [AppliedAt] datetimeoffset NOT NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_N5Applications] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_N5Applications_Status] CHECK ([Status] IN (0, 1, 2, 3)),
        CONSTRAINT [CK_N5Applications_Utc] CHECK (DATEPART(TZOFFSET, [AppliedAt]) = 0),
        CONSTRAINT [FK_N5Applications_N5CandidateProfiles_CandidateProfileId] FOREIGN KEY ([CandidateProfileId]) REFERENCES [N5CandidateProfiles] ([Id]),
        CONSTRAINT [FK_N5Applications_N5JobPostings_JobPostingId] FOREIGN KEY ([JobPostingId]) REFERENCES [N5JobPostings] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE TABLE [N5JobSkills] (
        [JobPostingId] int NOT NULL,
        [SkillId] int NOT NULL,
        CONSTRAINT [PK_N5JobSkills] PRIMARY KEY ([JobPostingId], [SkillId]),
        CONSTRAINT [FK_N5JobSkills_N5JobPostings_JobPostingId] FOREIGN KEY ([JobPostingId]) REFERENCES [N5JobPostings] ([Id]),
        CONSTRAINT [FK_N5JobSkills_N5Skills_SkillId] FOREIGN KEY ([SkillId]) REFERENCES [N5Skills] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE INDEX [IX_N5Applications_CandidateProfileId] ON [N5Applications] ([CandidateProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_N5Applications_JobPostingId_CandidateProfileId] ON [N5Applications] ([JobPostingId], [CandidateProfileId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE INDEX [IX_N5Applications_JobPostingId_Status] ON [N5Applications] ([JobPostingId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_N5CandidateProfiles_UserId] ON [N5CandidateProfiles] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE INDEX [IX_N5CandidateSkills_SkillId] ON [N5CandidateSkills] ([SkillId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_N5Companies_EmployerUserId] ON [N5Companies] ([EmployerUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE INDEX [IX_N5JobPostings_CompanyId_Status] ON [N5JobPostings] ([CompanyId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE INDEX [IX_N5JobPostings_Status_Deadline_PublishedAt] ON [N5JobPostings] ([Status], [Deadline], [PublishedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE INDEX [IX_N5JobSkills_SkillId] ON [N5JobSkills] ([SkillId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_N5Skills_NormalizedName] ON [N5Skills] ([NormalizedName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027181133_N5InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261027181133_N5InitialCreate', N'10.0.0');
END;

COMMIT;
GO

