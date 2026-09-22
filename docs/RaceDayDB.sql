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

-- User table
CREATE TABLE [User] (
    UserId          INT IDENTITY(1,1) PRIMARY KEY,
    FirstName       NVARCHAR(50)  NOT NULL,
    LastName        NVARCHAR(50)  NOT NULL,
    Email           NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash    NVARCHAR(255) NOT NULL,
    PhoneNumber     NVARCHAR(20)  NULL,
    ProfileImageUrl NVARCHAR(500) NULL,
    RoleId          INT           NOT NULL,
    CreatedAt       DATETIME      NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (RoleId) REFERENCES Role(RoleId)
);
GO

-- Event table
CREATE TABLE Event (
    EventId        INT IDENTITY(1,1) PRIMARY KEY,
    Name           NVARCHAR(100) NOT NULL,
    Description    NVARCHAR(500) NULL,
    EventDate      DATETIME      NOT NULL,
    Location       NVARCHAR(150) NOT NULL,
    Distance       DECIMAL(6,2)  NOT NULL,
    EventTypeId    INT           NOT NULL,
    OrganiserId    INT           NOT NULL,
    BannerImageUrl NVARCHAR(500) NULL,
    CreatedAt      DATETIME      NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (EventTypeId) REFERENCES EventType(EventTypeId),
    FOREIGN KEY (OrganiserId) REFERENCES [User](UserId)
);
GO

-- Category table
CREATE TABLE Category (
    CategoryId      INT IDENTITY(1,1) PRIMARY KEY,
    EventId         INT           NOT NULL,
    Name            NVARCHAR(50)  NOT NULL,
    Description     NVARCHAR(255) NULL,
    MaxParticipants INT           NULL,
    EntryFee        DECIMAL(8,2)  NOT NULL DEFAULT 0,
    FOREIGN KEY (EventId) REFERENCES Event(EventId) ON DELETE CASCADE
);
GO

-- Enrolment table
CREATE TABLE Enrolment (
    EnrolmentId   INT IDENTITY(1,1) PRIMARY KEY,
    UserId        INT           NOT NULL,
    EventId       INT           NOT NULL,
    CategoryId    INT           NOT NULL,
    EnrolmentDate DATETIME      NOT NULL DEFAULT GETDATE(),
    Status        NVARCHAR(20)  NOT NULL DEFAULT 'Confirmed',
    FOREIGN KEY (UserId)     REFERENCES [User](UserId),
    FOREIGN KEY (EventId)    REFERENCES Event(EventId),
    FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId),
    UNIQUE (UserId, EventId, CategoryId),
    CHECK (Status IN ('Confirmed','Pending','Cancelled'))
);
GO