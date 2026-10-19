USE RaceDayDB;
GO

-- 1. Clear data and reset IDs back to 1
DELETE FROM Result;
DELETE FROM Enrolment;
DELETE FROM Category;
DELETE FROM Event;
DELETE FROM [User];
DELETE FROM EventType;
DELETE FROM Role;

DBCC CHECKIDENT ('Result', RESEED, 0);
DBCC CHECKIDENT ('Enrolment', RESEED, 0);
DBCC CHECKIDENT ('Category', RESEED, 0);
DBCC CHECKIDENT ('Event', RESEED, 0);
DBCC CHECKIDENT ('[User]', RESEED, 0);
DBCC CHECKIDENT ('EventType', RESEED, 0);
DBCC CHECKIDENT ('Role', RESEED, 0);
GO

-- 2. Insert Roles and EventTypes
INSERT INTO Role (RoleName) VALUES ('Organiser'), ('Participant');
INSERT INTO EventType (TypeName) VALUES ('Run'), ('Walk'), ('Cycle');
GO

-- 3. Insert Users
INSERT INTO [User] (FirstName, LastName, Email, PasswordHash, PhoneNumber, RoleId, CreatedAt) VALUES
    ('Thabo',  'Mokoena', 'thabo.organiser@raceday.co.za',   'HASH_ORG_1', '0821110001', 1, GETDATE()),
    ('Lerato', 'Nkosi',   'lerato.organiser@raceday.co.za',  'HASH_ORG_2', '0821110002', 1, GETDATE()),
    ('Sipho',  'Dlamini', 'sipho.participant@raceday.co.za', 'HASH_PAR_1', '0821110003', 2, GETDATE()),
    ('Aisha',  'Patel',   'aisha.participant@raceday.co.za', 'HASH_PAR_2', '0821110004', 2, GETDATE());
GO

-- 4. Insert Events
INSERT INTO Event (Name, Description, EventDate, Location, Distance, EventTypeId, OrganiserId, CreatedAt) VALUES
    ('Comrades Warm-Up 10K',     'A 10km road run to prepare for Comrades.',   '2026-04-12 06:00:00', 'Durban Beachfront, KZN', 10.00, 1, 1, GETDATE()),
    ('Cape Town Cycle Tour Lite','Family-friendly 30km cycle.',               '2026-05-03 07:30:00', 'Green Point, Cape Town', 30.00, 3, 2, GETDATE()),
    ('Soweto Charity Walk',      'A 5km walk raising funds for local schools.','2026-06-21 08:00:00', 'Orlando Stadium, Soweto', 5.00, 2, 1, GETDATE());
GO

-- 5. Insert Categories
INSERT INTO Category (EventId, Name, Description, MaxParticipants, EntryFee) VALUES
    (1, 'Senior (18-39)', 'Open senior category for the 10K',   500, 150.00),
    (1, 'Veteran (40+)',  'Veteran category for the 10K',       300, 150.00),
    (2, 'Open 30km',      'Open category for the cycle tour',   400, 200.00),
    (2, 'Junior (U18)',   'Junior category for the cycle tour', 150, 100.00),
    (3, 'Open 5km Walk',  'General walking category',          1000,  50.00);
GO

-- 6. Insert Enrolments
INSERT INTO Enrolment (UserId, EventId, CategoryId, Status, EnrolmentDate) VALUES
    (3, 1, 1, 'Confirmed', GETDATE()),
    (4, 1, 2, 'Confirmed', GETDATE()),
    (3, 3, 5, 'Confirmed', GETDATE()),
    (4, 2, 3, 'Pending', GETDATE());
GO

-- 7. Insert Results
INSERT INTO Result (EnrolmentId, FinishTime, FinishingPosition, CapturedAt) VALUES
    (1, '00:48:12', 47, GETDATE()),
    (2, '00:55:30', 118, GETDATE()),
    (3, '00:38:05', 12, GETDATE());
GO

-- 8. Update passwords
UPDATE [User] SET PasswordHash = 'n1CVL5j6x5T8U2yJ8bK6sV5g8yT8K7M=';
GO