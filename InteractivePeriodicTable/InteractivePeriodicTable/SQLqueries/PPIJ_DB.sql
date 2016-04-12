CREATE TABLE DidYouKnow
(
	ID			 INTEGER         PRIMARY KEY IDENTITY(1,1),
	Fact		 NVARCHAR(1024)  NOT NULL
);

CREATE TABLE QuizWith4Ans
(
	ID           INTEGER         PRIMARY KEY IDENTITY(1,1),
	Question     NVARCHAR(200)   NOT NULL,
	Answer       TINYINT         NOT NULL,
	A1           NVARCHAR(100)    NOT NULL,
	A2			 NVARCHAR(100)    NOT NULL,
	A3			 NVARCHAR(100)    NOT NULL,
	A4			 NVARCHAR(100)    NOT NULL
);

CREATE TABLE QuizYesNo
(
	ID           INTEGER         PRIMARY KEY IDENTITY(1,1),
	Question     NVARCHAR(200)   NOT NULL,
	Answer       TINYINT         NOT NULL,
	A1           NVARCHAR(50)    NOT NULL,
	A2			 NVARCHAR(50)    NOT NULL,
);

CREATE TABLE QuizPictures
(
	ID           INTEGER         PRIMARY KEY IDENTITY(1,1),
	ImagePath    NVARCHAR(200)   NOT NULL,
	Answer       NVARCHAR(100)   NOT NULL,
);

CREATE TABLE UserScore
(
	UserName    NVARCHAR(20)    PRIMARY KEY,
	Score       INTEGER         NOT NULL
);
GO