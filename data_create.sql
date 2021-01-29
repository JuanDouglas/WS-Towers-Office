﻿CREATE TABLE [File] (
[ID] INTEGER IDENTITY PRIMARY KEY NOT NULL,
[FileType] INT NOT NULL,
[FileName] VARCHAR(300) UNIQUE NOT NULL,
[Width] INTEGER NULL,
[Height] INTEGER NULL,
[Leaght] INTEGER NOT NULL
);

CREATE TABLE [Team] (
[ID] INTEGER PRIMARY KEY IDENTITY NOT NULL,
[Name] VARCHAR(100) UNIQUE NOT NULL,
[Icon] INTEGER NOT NULL,
[Description] VARCHAR(1000) NOT NULL,
FOREIGN KEY ([Icon]) REFERENCES [File]([ID])
);