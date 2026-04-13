USE [master];
GO

IF DB_ID(N'magazine_videoigr') IS NULL
BEGIN
    CREATE DATABASE [magazine_videoigr];
END;
GO

SELECT name
FROM sys.databases
WHERE name = N'magazine_videoigr';
GO
