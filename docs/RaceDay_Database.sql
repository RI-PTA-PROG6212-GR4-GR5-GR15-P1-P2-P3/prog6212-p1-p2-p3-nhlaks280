-- RaceDay Database Script
-- PROG6212 Part 1: System Planning and Database

CREATE DATABASE RaceDay;
GO

USE RaceDay;
GO

-- =============================================
-- 1. USER
-- =============================================
CREATE TABLE [USER]
(
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Role VARCHAR(20) NOT NULL DEFAULT 'Participant',

    CONSTRAINT CK_USER_Role
        CHECK (Role IN ('Organiser', 'Participant'))
);
GO

-- =============================================
-- 2. EVENT
-- =============================================
CREATE TABLE [EVENT]
(
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserID INT NOT NULL,
    EventName VARCHAR(100) NOT NULL,
    Description VARCHAR(500),
    EventDate DATE NOT NULL,
    Location VARCHAR(200) NOT NULL,

    CONSTRAINT FK_EVENT_Organiser
        FOREIGN KEY (OrganiserID)
        REFERENCES [USER](UserID)
);
GO

-- =============================================
-- 3. CATEGORY
-- =============================================
CREATE TABLE CATEGORY
(
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName VARCHAR(100) NOT NULL UNIQUE,
    Description VARCHAR(300)
);
GO

-- =============================================
-- 4. EVENTCATEGORY
-- =============================================
CREATE TABLE EVENTCATEGORY
(
    EventCategoryID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,

    CONSTRAINT FK_EVENTCATEGORY_Event
        FOREIGN KEY (EventID)
        REFERENCES [EVENT](EventID),

    CONSTRAINT FK_EVENTCATEGORY_Category
        FOREIGN KEY (CategoryID)
        REFERENCES CATEGORY(CategoryID),

    CONSTRAINT UQ_EVENTCATEGORY_Event_Category
        UNIQUE (EventID, CategoryID)
);
GO

-- =============================================
-- 5. ENROLLMENT
-- =============================================
CREATE TABLE ENROLLMENT
(
    EnrollmentID INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID INT NOT NULL,
    EventCategoryID INT NOT NULL,
    EnrollmentDate DATE NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_ENROLLMENT_Participant
        FOREIGN KEY (ParticipantID)
        REFERENCES [USER](UserID),

    CONSTRAINT FK_ENROLLMENT_EventCategory
        FOREIGN KEY (EventCategoryID)
        REFERENCES EVENTCATEGORY(EventCategoryID),

    CONSTRAINT UQ_ENROLLMENT_Participant_EventCategory
        UNIQUE (ParticipantID, EventCategoryID)
);
GO

-- =============================================
-- 6. RESULT
-- =============================================
CREATE TABLE RESULT
(
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    EnrollmentID INT NOT NULL UNIQUE,
    FinishTime TIME NOT NULL,
    Position INT NOT NULL,

    CONSTRAINT FK_RESULT_Enrollment
        FOREIGN KEY (EnrollmentID)
        REFERENCES ENROLLMENT(EnrollmentID),

    CONSTRAINT CK_RESULT_Position
        CHECK (Position > 0)
);
GO

-- =============================================
-- SAMPLE USERS
-- 2 Organisers + 2 Participants
-- =============================================
INSERT INTO [USER]
    (FirstName, LastName, Email, PasswordHash, Role)
VALUES
    ('James', 'Mokoena', 'james@raceday.co.za', 'HASH_ORGANISER_001', 'Organiser'),
    ('Lerato', 'Naidoo', 'lerato@raceday.co.za', 'HASH_ORGANISER_002', 'Organiser'),
    ('Thabo', 'Dlamini', 'thabo@example.com', 'HASH_PARTICIPANT_001', 'Participant'),
    ('Ayesha', 'Khan', 'ayesha@example.com', 'HASH_PARTICIPANT_002', 'Participant');
GO

-- =============================================
-- SAMPLE EVENTS
-- 3 Events
-- =============================================
INSERT INTO [EVENT]
    (OrganiserID, EventName, Description, EventDate, Location)
VALUES
    (1, 'Johannesburg City Run', 'A city road running event.', '2026-10-10', 'Johannesburg'),
    (2, 'Pretoria Trail Challenge', 'A challenging outdoor trail event.', '2026-11-07', 'Pretoria'),
    (1, 'Cape Town Fun Run', 'A family-friendly running event.', '2026-12-05', 'Cape Town');
GO

-- =============================================
-- SAMPLE CATEGORIES
-- =============================================
INSERT INTO CATEGORY
    (CategoryName, Description)
VALUES
    ('10KM Road Race', 'A ten kilometre road race.'),
    ('Trail Run', 'A running event held on a trail.'),
    ('5KM Fun Run', 'A five kilometre recreational run.'),
    ('21KM Half Marathon', 'A twenty-one kilometre road race.');
GO

-- =============================================
-- LINK EVENTS TO CATEGORIES
-- =============================================
INSERT INTO EVENTCATEGORY
    (EventID, CategoryID)
VALUES
    (1, 1),
    (1, 4),
    (2, 2),
    (3, 3);
GO

-- =============================================
-- SAMPLE ENROLLMENTS
-- =============================================
INSERT INTO ENROLLMENT
    (ParticipantID, EventCategoryID, EnrollmentDate)
VALUES
    (3, 1, '2026-09-01'),
    (3, 3, '2026-09-02'),
    (4, 2, '2026-09-02'),
    (4, 4, '2026-09-03');
GO

-- =============================================
-- SAMPLE RESULTS
-- =============================================
INSERT INTO RESULT
    (EnrollmentID, FinishTime, Position)
VALUES
    (1, '00:52:35', 3),
    (3, '01:15:20', 5);
GO

-- =============================================
-- VERIFY THE DATA
-- =============================================
SELECT * FROM [USER];
SELECT * FROM [EVENT];
SELECT * FROM CATEGORY;
SELECT * FROM EVENTCATEGORY;
SELECT * FROM ENROLLMENT;
SELECT * FROM RESULT;
GO