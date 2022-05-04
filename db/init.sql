DROP TABLE IF EXISTS Event_Attachment CASCADE;
DROP TABLE IF EXISTS Event_Question CASCADE;
DROP TABLE IF EXISTS Completed_Event CASCADE;
DROP TABLE IF EXISTS Recurrent_Event_Setting CASCADE;
DROP TABLE IF EXISTS Event_Participant CASCADE;
DROP TABLE IF EXISTS Event CASCADE;
DROP TABLE IF EXISTS Calendar_Participant CASCADE;
DROP TABLE IF EXISTS Calendar CASCADE;
DROP TABLE IF EXISTS Notification_Subscription CASCADE;
DROP TABLE IF EXISTS User_Token CASCADE;
DROP TABLE IF EXISTS "user" CASCADE;

CREATE TABLE "user"
(
    Id            UUID                    NOT NULL PRIMARY KEY,
    Name          VARCHAR(50)             NOT NULL,
    Email         VARCHAR(50)             NOT NULL,
    Position      VARCHAR(50),
    Department    VARCHAR(50),
    Register_Date TIMESTAMP DEFAULT now() NOT NULL,
    Password_Hash BYTEA                   NOT NULL,
    Is_Deleted    BOOL      DEFAULT FALSE NOT NULL
);

CREATE TABLE User_Token
(
    User_Id       UUID         NOT NULL REFERENCES "user" ON DELETE CASCADE,
    Refresh_Token VARCHAR(400) NOT NULL,
    Valid_Until   TIMESTAMP    NOT NULL,
    PRIMARY KEY (User_Id, Refresh_Token)
);

CREATE TABLE Notification_Subscription
(
    User_Id UUID         NOT NULL REFERENCES "user" ON DELETE CASCADE,
    Url     VARCHAR(500) NOT NULL,
    Auth    VARCHAR(200) NOT NULL,
    P256dh  VARCHAR(200) NOT NULL,
    PRIMARY KEY (Url)
);

CREATE TABLE Calendar
(
    Id            UUID                    NOT NULL PRIMARY KEY,
    Name          VARCHAR(50)             NOT NULL,
    Description   VARCHAR(200)            NOT NULL,
    Creation_Date TIMESTAMP DEFAULT now() NOT NULL,
    Is_Deleted    BOOL      DEFAULT FALSE NOT NULL,
    UNIQUE (Name)
);

CREATE TABLE Calendar_Participant
(
    Id          UUID                    NOT NULL PRIMARY KEY,
    Calendar_Id UUID                    NOT NULL REFERENCES Calendar ON DELETE CASCADE,
    User_Id     UUID                    NOT NULL REFERENCES "user" ON DELETE CASCADE,
    Role        INT       DEFAULT 0     NOT NULL,
    Join_Date   TIMESTAMP DEFAULT now() NOT NULL,
    Is_Deleted  BOOL      DEFAULT FALSE NOT NULL,
    UNIQUE (Calendar_Id, User_Id)
);

CREATE TABLE Event
(
    Id            UUID                    NOT NULL PRIMARY KEY,
    Calendar_Id   UUID                    NOT NULL REFERENCES Calendar ON DELETE CASCADE,
    Event_Type    INT                     NOT NULL,
    Priority      INT       DEFAULT 0     NOT NULL,
    Name          VARCHAR(100)            NOT NULL,
    Description   TEXT,
    Place         VARCHAR(200),
    Start_Time    TIMESTAMP               NOT NULL,
    End_Time      TIMESTAMP               NOT NULL,
    Is_Private    BOOL                    NOT NULL,
    Is_Repeated   BOOL      DEFAULT FALSE NOT NULL,
    Is_Completed  BOOL      DEFAULT FALSE NOT NULL,
    Creation_Time TIMESTAMP DEFAULT now() NOT NULL
);

CREATE TABLE Event_Participant
(
    Id                        UUID               NOT NULL PRIMARY KEY,
    Event_Id                  UUID               NOT NULL REFERENCES Event ON DELETE CASCADE,
    Calendar_Participant_Id   UUID               NOT NULL REFERENCES Calendar_Participant ON DELETE CASCADE,
    Status_Type               INT  DEFAULT 0     NOT NULL,
    Start_Notification_Before INTERVAL,
    Is_Deleted                BOOL DEFAULT FALSE NOT NULL,
    Role                      INT                NOT NULL,
    UNIQUE (Event_Id, Calendar_Participant_Id)
);

CREATE TABLE Recurrent_Event_Setting
(
    Event_Id UUID        NOT NULL REFERENCES Event ON DELETE CASCADE,
    Key      VARCHAR(20) NOT NULL,
    Value    BIGINT      NOT NULL,
    PRIMARY KEY (Event_Id, Key, Value)
);

-- Будущий функционал.
CREATE TABLE Completed_Event
(
    Event_Id    UUID NOT NULL REFERENCES Event ON DELETE CASCADE,
    Repeat_Num  INT  NOT NULL,
    Reason_Type INT  NOT NULL,
    PRIMARY KEY (Event_Id, Repeat_Num)
);

-- Будущий функционал.
CREATE TABLE Event_Chat
(
    Id             UUID    NOT NULL PRIMARY KEY,
    Event_Id       UUID    NOT NULL REFERENCES Event ON DELETE CASCADE,
    Event_Participant_Id  UUID    NOT NULL REFERENCES Event_Participant ON DELETE CASCADE,
    Value          VARCHAR NOT NULL,
    Parent_Id      UUID    REFERENCES Event_Question ON DELETE CASCADE
);

-- Будущий функционал.
CREATE TABLE Event_Attachment
(
    Event_Id UUID        NOT NULL REFERENCES Event ON DELETE CASCADE,
    Name     VARCHAR(50) NOT NULL,
    Value    BYTEA       NOT NULL,
    PRIMARY KEY (Event_Id, Name)
);