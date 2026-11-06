SELECT WeightClass, SUM(Wins) AS TotalWins
FROM Athletes
GROUP BY WeightClass;


SELECT WeightClass, SUM(Wins) AS TotalWins
FROM Athletes
GROUP BY WeightClass
HAVING SUM(Wins) > 10;