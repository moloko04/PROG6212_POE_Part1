-- RACEDAY DATABASE SCRIPT
-- PROG6212 - Part 1
-- Student: Moloko Dineo Mothemela
-- Student Number: ST10460465


CREATE DATABASE RaceDay;
GO

USE RaceDay; 
GO

CREATE TABLE [User] (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE, 
    Role VARCHAR(20) NOT NULL CHECK (Role IN ('Organiser', 'Participant', 'Admin')), 
    CreatedDate DATETIME DEFAULT GETDATE()
);

CREATE TABLE Event (
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    EventName VARCHAR(100) NOT NULL,
    EventDate DATETIME NOT NULL,
    Location VARCHAR(200) NOT NULL,
    Description TEXT,
    Status VARCHAR(20) DEFAULT 'Upcoming' CHECK (Status IN ('Upcoming', 'Ongoing', 'Completed', 'Cancelled')),
    OrganiserID INT NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (OrganiserID) REFERENCES [User](UserID)
);

CREATE TABLE Category (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName VARCHAR(50) NOT NULL,
    Description VARCHAR(200),
    EventID INT NOT NULL,
    FOREIGN KEY (EventID) REFERENCES Event(EventID) 
);

CREATE TABLE Enrolment (
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    ParticipantID INT NOT NULL,
    EnrolmentDate DATETIME DEFAULT GETDATE(),
    Status VARCHAR(20) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Confirmed', 'Cancelled')),
    FOREIGN KEY (EventID) REFERENCES Event(EventID),
    FOREIGN KEY (ParticipantID) REFERENCES [User](UserID)
);

CREATE TABLE Result (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentID INT NOT NULL UNIQUE,
    FinishTime TIME,
    Position INT,
    PointsEarned INT,
    IsDisqualified BIT DEFAULT 0,
    FOREIGN KEY (EnrolmentID) REFERENCES Enrolment(EnrolmentID)
);

CREATE TABLE Prize (
    PrizeID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,
    PrizeName VARCHAR(100) NOT NULL,
    PrizeValue DECIMAL(10,2),
    PositionAwarded INT,
    FOREIGN KEY (EventID) REFERENCES Event(EventID),
    FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
);

INSERT INTO [User] (Username, PasswordHash, FullName, Email, Role)
VALUES 
('thabo_organiser', 'hashed_password_123', 'Thabo Mokoena', 'thabo@raceday.com', 'Organiser'),
('nosipho_organiser', 'hashed_password_456', 'Nosipho Ndlovu', 'nosipho@raceday.com', 'Organiser'),
('sipho_participant', 'hashed_password_789', 'Sipho Dlamini', 'sipho@email.com', 'Participant'),
('zinhle_participant', 'hashed_password_101', 'Zinhle Khumalo', 'zinhle@email.com', 'Participant'),
('lebo_participant', 'hashed_password_202', 'Lebo Ramaphosa', 'lebo@email.com', 'Participant'),
('nomsa_participant', 'hashed_password_303', 'Nomsa Mthembu', 'nomsa@email.com', 'Participant');

INSERT INTO Event (EventName, EventDate, Location, Description, Status, OrganiserID)
VALUES 
('Soweto Marathon', '2026-12-15 06:00:00', 'Soweto, Johannesburg', 'Annual Soweto Marathon featuring full and half marathon categories', 'Upcoming', 1),
('Cape Town Cycle Tour', '2026-03-08 07:00:00', 'Cape Town', 'World-famous cycle tour with multiple distance categories', 'Upcoming', 2);

INSERT INTO Category (CategoryName, Description, EventID)
VALUES 
('Full Marathon (42km)', 'Standard full marathon distance', 1),
('Half Marathon (21km)', 'Half marathon distance', 1),
('10km Fun Run', 'Fun run for all ages', 1),
('Elite (100km)', 'Elite competitive category', 2),
('Amateur (100km)', 'Amateur competitive category', 2),
('Family Ride (25km)', 'Family friendly ride', 2);

INSERT INTO Enrolment (EventID, ParticipantID, Status)
VALUES 
(1, 3, 'Confirmed'),
(1, 4, 'Pending'),
(2, 3, 'Confirmed');

INSERT INTO Result (EnrolmentID, FinishTime, Position, PointsEarned, IsDisqualified)
VALUES 
(1, '02:45:30', 5, 95, 0),
(3, '03:30:15', 12, 88, 0);

INSERT INTO Prize (EventID, CategoryID, PrizeName, PrizeValue, PositionAwarded)
VALUES 
(1, 1, 'Gold Medal', 5000.00, 1),
(1, 1, 'Silver Medal', 3000.00, 2),
(1, 1, 'Bronze Medal', 1500.00, 3),
(1, 2, 'Gold Medal', 2500.00, 1),
(1, 2, 'Silver Medal', 1500.00, 2),
(1, 2, 'Bronze Medal', 750.00, 3),
(2, 4, 'Gold Trophy', 10000.00, 1),
(2, 4, 'Silver Trophy', 5000.00, 2),
(2, 4, 'Bronze Trophy', 2500.00, 3);

SELECT * FROM [User];
SELECT * FROM Event;
SELECT * FROM Category;
SELECT * FROM Enrolment;
SELECT * FROM Result;
SELECT * FROM Prize;
