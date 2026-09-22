-- RaceDay Database Script
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

-- Result table
CREATE TABLE Result (
    ResultId          INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentId       INT      NOT NULL UNIQUE,
    FinishTime        TIME     NOT NULL,
    FinishingPosition INT      NOT NULL,
    CapturedAt        DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (EnrolmentId) REFERENCES Enrolment(EnrolmentId) ON DELETE CASCADE,
    CHECK (FinishingPosition > 0)
);
GO

-- Seed Roles
INSERT INTO Role (RoleName) VALUES
    ('Organiser'),
    ('Participant');

-- Seed Event Types
INSERT INTO EventType (TypeName) VALUES
    ('Run'),
    ('Walk'),
    ('Cycle');
GO

-- Seed Users
INSERT INTO [User] (FirstName, LastName, Email, PasswordHash, PhoneNumber, RoleId) VALUES
    ('Thabo',  'Mokoena', 'thabo.organiser@raceday.co.za',   'HASH_ORG_1', '0821110001', 1),
    ('Lerato', 'Nkosi',   'lerato.organiser@raceday.co.za',  'HASH_ORG_2', '0821110002', 1),
    ('Sipho',  'Dlamini', 'sipho.participant@raceday.co.za', 'HASH_PAR_1', '0821110003', 2),
    ('Aisha',  'Patel',   'aisha.participant@raceday.co.za', 'HASH_PAR_2', '0821110004', 2);

-- Seed Events
INSERT INTO Event (Name, Description, EventDate, Location, Distance, EventTypeId, OrganiserId) VALUES
    ('Comrades Warm-Up 10K',     'A 10km road run to prepare for Comrades.',   '2026-04-12 06:00:00', 'Durban Beachfront, KZN', 10.00, 1, 1),
    ('Cape Town Cycle Tour Lite','Family-friendly 30km cycle.',               '2026-05-03 07:30:00', 'Green Point, Cape Town', 30.00, 3, 2),
    ('Soweto Charity Walk',      'A 5km walk raising funds for local schools.','2026-06-21 08:00:00', 'Orlando Stadium, Soweto', 5.00, 2, 1);
GO

-- Seed Categories
INSERT INTO Category (EventId, Name, Description, MaxParticipants, EntryFee) VALUES
    (1, 'Senior (18-39)', 'Open senior category for the 10K',   500, 150.00),
    (1, 'Veteran (40+)',  'Veteran category for the 10K',       300, 150.00),
    (2, 'Open 30km',      'Open category for the cycle tour',   400, 200.00),
    (2, 'Junior (U18)',   'Junior category for the cycle tour', 150, 100.00),
    (3, 'Open 5km Walk',  'General walking category',          1000,  50.00);

-- Seed Enrolments
INSERT INTO Enrolment (UserId, EventId, CategoryId, Status) VALUES
    (3, 1, 1, 'Confirmed'),
    (4, 1, 2, 'Confirmed'),
    (3, 3, 5, 'Confirmed'),
    (4, 2, 3, 'Pending');

-- Seed Results
INSERT INTO Result (EnrolmentId, FinishTime, FinishingPosition) VALUES
    (1, '00:48:12', 47),
    (2, '00:55:30', 118),
    (3, '00:38:05', 12);

PRINT 'RaceDayDB created and seeded successfully.';
GO