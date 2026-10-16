Create database RaceDayAU;

use RaceDayAU;

CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Email VARCHAR(100),
    PasswordHash VARCHAR(255),
    PhoneNumber VARCHAR(20),
    physicalAddress VARCHAR(255),
    UserRole VARCHAR(20) CHECK (UserRole IN ('Organiser', 'Participant'))
);

CREATE TABLE Events (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    OrganizerId INT,
    EventName VARCHAR(100) ,
    EventLocation VARCHAR(100),
    EventDate DATE,
    EventDescription TEXT,
    FOREIGN KEY (OrganizerId) REFERENCES Users(UserId)
);

CREATE TABLE EventTypes (
    EventTypeCode INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT,
    EventTypeName VARCHAR(50),
    DistanceKM DECIMAL(5,2),
    EntranceFee DECIMAL(10,2) DEFAULT 0.00,
    MaxParticipants INT,
    FOREIGN KEY (EventId) REFERENCES Events(EventId)
);

CREATE TABLE RouteMap (
    RouteId INT IDENTITY(1,1) PRIMARY KEY,
    EventTypeCode INT,
    MapDate DATE DEFAULT GETDATE(),
    StartPoint VARCHAR(100),
    EndPoint VARCHAR(100),
    RouteDescription TEXT,
    FOREIGN KEY (EventTypeCode) REFERENCES EventTypes(EventTypeCode) 
);

CREATE TABLE RaceEntrance (
    EntranceId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    EventTypeCode INT,
    ChipNumber VARCHAR(50),
    PaymentStatus VARCHAR(20) DEFAULT 'Pending' CHECK (PaymentStatus IN ('Pending', 'Paid', 'Failed')),
    MedicalNotes TEXT,
    EmergencyContactNum VARCHAR(20),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (EventTypeCode) REFERENCES EventTypes(EventTypeCode)
);

CREATE TABLE Results (
    ResultId INT IDENTITY(1,1) PRIMARY KEY,
    EntranceId INT,
    FinishStatus VARCHAR(20) DEFAULT 'Finished' CHECK (FinishStatus IN ('Finished', 'DNF', 'DNS', 'Disqualified')),
    OverallRank INT,
    CategoryRank INT,
    AgeGroupRank INT,
    FOREIGN KEY (EntranceId) REFERENCES RaceEntrance(EntranceId)
);

INSERT INTO Users (FirstName, LastName, Email, PasswordHash, PhoneNumber, physicalAddress, UserRole) 
VALUES 
('Thabo', 'Mbeki', 'thabo@raceday.co.za', 'hash123', '0821234567', '12 Long St, Cape Town', 'Organiser'),
('Sarah', 'Jones', 'sarah@events.co.za', 'hash123', '0719876543', '88 Vilakazi St, Soweto', 'Organiser'),
('Sipho', 'Nkosi', 'sipho@runner.com', 'hash123', '0834445555', '45 Main Rd, Durban', 'Participant'),
('Emma', 'Smith', 'emma@runner.com', 'hash123', '0723332222', '10 Park Ln, Pretoria', 'Participant');

INSERT INTO Events (OrganizerId, EventName, EventLocation, EventDate, EventDescription) 
VALUES 
(1, 'Cape Town Cycle Tour 2027', 'Cape Town Stadium', '2027-03-14', 'Iconic cycling event around the peninsula.'),
(2, 'Soweto Marathon', 'FNB Stadium, Johannesburg', '2026-11-01', 'The Peoples Race through historical Soweto.'),
(1, 'Two Oceans Marathon', 'Newlands, Cape Town', '2027-04-03', 'The worlds most beautiful marathon.');

INSERT INTO EventTypes (EventId, EventTypeName, DistanceKM, EntranceFee, MaxParticipants) 
VALUES 
(1, 'Standard Route', 109.00, 850.00, 35000), -- CT Cycle Tour
(2, 'Full Marathon', 42.20, 400.00, 10000), -- Soweto
(2, 'Half Marathon', 21.10, 300.00, 15000), -- Soweto
(3, 'Ultra Marathon', 56.00, 750.00, 14000); -- Two Oceans

INSERT INTO RouteMap (EventTypeCode, StartPoint, EndPoint, RouteDescription) 
VALUES 
(1, 'Grand Parade', 'Green Point', 'Scenic route via Cape Point.'),
(2, 'FNB Stadium', 'FNB Stadium', 'Route passing Vilakazi street and Hector Pieterson memorial.');

INSERT INTO RaceEntrance (UserId, EventTypeCode, ChipNumber, PaymentStatus, MedicalNotes, EmergencyContactNum) 
VALUES 
(3, 2, 'CHIP-9901', 'Paid', 'None', '0825556666'), -- Sipho enters Soweto Full
(4, 3, 'CHIP-9902', 'Paid', 'Asthma', '0831112222'), -- Emma enters Soweto Half
(3, 4, 'CHIP-9903', 'Pending', 'None', '0825556666'); -- Sipho enters Two Oceans Ultra

INSERT INTO Results (EntranceId, FinishStatus, OverallRank, CategoryRank, AgeGroupRank) 
VALUES 
(1, 'Finished', 145, 30, 5),
(2, 'Finished', 42, 10, 2);
