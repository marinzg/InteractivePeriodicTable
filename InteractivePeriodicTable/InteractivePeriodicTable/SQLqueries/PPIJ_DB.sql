CREATE DATABASE PPIJ;
GO

USE PPIJ;
GO

CREATE TABLE did_you_know
(
	ID			 INTEGER         IDENTITY(1,1),
	Fact		 NVARCHAR(100)   NOT NULL
);

CREATE TABLE quiz
(
	ID           INTEGER         IDENTITY(1,1),
	Question     NVARCHAR(100)   NOT NULL,
	Answer       TINYINT         NOT NULL,
	A1           NVARCHAR(50)    NOT NULL,
	A2			 NVARCHAR(50)    NOT NULL,
	A3			 NVARCHAR(50)    NOT NULL,
	A4			 NVARCHAR(50)    NOT NULL
);

CREATE TABLE element
(
	Name		 VARCHAR(3)      NOT NULL,
	Info		 NVARCHAR(4000)  NOT NULL,
	File_path	 VARCHAR(100)    NOT NULL,
	Type		 VARCHAR(30)	 NOT NULL
);
GO

CREATE TABLE user_score
(
	User_name   NVARCHAR(10)    NOT NULL,
	Password    NVARCHAR(10)    NOT NULL,
	Score       INTEGER         NOT NULL
);
GO

CREATE UNIQUE INDEX UI_dyk ON did_you_know (ID);

CREATE UNIQUE INDEX UI_q ON quiz (ID);

CREATE NONCLUSTERED INDEX I_e ON element (Type);

CREATE UNIQUE INDEX PK_usr ON user_score (User_name);

ALTER TABLE element ADD CONSTRAINT PK_element_Name PRIMARY KEY CLUSTERED (Name);
GO