
CREATE TABLE DidYouKnow
(
	ID			 INTEGER         IDENTITY(1,1),
	Fact		 NVARCHAR(100)   NOT NULL
);

CREATE TABLE QuizWith4Ans
(
	ID          INTEGER    PRIMARY KEY      IDENTITY(1,1),
	Question     NVARCHAR(100)   NOT NULL,
	Answer       TINYINT         NOT NULL,
	A1           NVARCHAR(50)    NOT NULL,
	A2			 NVARCHAR(50)    NOT NULL,
	A3			 NVARCHAR(50)    NOT NULL,
	A4			 NVARCHAR(50)    NOT NULL
);

CREATE TABLE QuizYesNo
(
	ID          INTEGER   PRIMARY KEY       IDENTITY(1,1),
	Question     NVARCHAR(100)   NOT NULL,
	Answer       TINYINT         NOT NULL,
	A1           NVARCHAR(50)    NOT NULL,
	A2			 NVARCHAR(50)    NOT NULL,
)
CREATE TABLE QuizPictures
(
	ID          INTEGER    PRIMARY KEY      IDENTITY(1,1),
	ImagePath     NVARCHAR(100)   NOT NULL,
	Answer       TINYINT         NOT NULL,
)

CREATE TABLE UserScore
(
	UserName   NVARCHAR(15) PRIMARY KEY    NOT NULL,
	Score       INTEGER         NOT NULL
);
