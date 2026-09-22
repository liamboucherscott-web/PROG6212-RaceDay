- RaceDay Database Script
-- PROG6212 - Programming 2B
-- Liam Scott - ST10467183

-- Drop the database if it already exists
IF DB_ID('RaceDayDB') IS NOT NULL
    DROP DATABASE RaceDayDB;
GO

-- Create the database
CREATE DATABASE RaceDayDB;
GO

-- Use the database
USE RaceDayDB;
GO

-- Role table
CREATE TABLE Role (
    RoleId   INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- EventType table
CREATE TABLE EventType (
    EventTypeId INT IDENTITY(1,1) PRIMARY KEY,
    TypeName    NVARCHAR(50) NOT NULL UNIQUE
);
GO
