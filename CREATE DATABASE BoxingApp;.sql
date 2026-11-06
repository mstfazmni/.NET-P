CREATE DATABASE BoxingApp;

CREATE TABLE Athletes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    WeightClass NVARCHAR(50) NOT NULL,
    Wins INT DEFAULT 0,
    Losses INT DEFAULT 0,
);

CREATE TABLE TrainingPlans (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AthleteId INT NOT NULL,
    PlanName NVARCHAR(100),
    FOREIGN KEY (AthleteId) REFERENCES Athletes(Id)
);


INSERT INTO Athletes (Name, WeightClass, Wins, Losses)
VALUES ('Mostafa', 'Lightheavy', 8, 2);

INSERT INTO Athletes (Name, WeightClass, Wins, Losses)
VALUES ('Floyd', 'Superlight', 50, 0);

INSERT INTO TrainingPlans (AthleteId, PlanName)
VALUES (1, 'Winning_Games');


SELECT * FROM Athletes;
SELECT * FROM TrainingPlans;

SELECT Athletes.Name, TrainingPlans.PlanName
From Athletes
LEFT JOIN TrainingPlans ON Athletes.Id = TrainingPlans.AthleteId;