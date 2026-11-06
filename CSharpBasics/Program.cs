// athlete class
public class Athlete
{
    // Properties
    public string Name { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; } 

    // Constructor
    public Athlete(string name)
    {
        Name = name;
        Wins = 0;
        Losses = 0;
    }

    // Methods
    public void RecordWin()
    {
        Wins++;
    }

    public void RecordLoss()
    {
        Losses++;
    }

    public virtual string GetRecord ()
    {
        return $"{Name} has {Wins} wins and {Losses} losses.";
    }
    
}
// pro athlete subclass
public class ProAthlete : Athlete
{
    // Constructor
    public ProAthlete(string name) : base(name)
    {
    }

    public override string GetRecord()
    {
        return $"{Name} is a pro athlete with {Wins} wins and {Losses} losses.";
    }
}

// AmateurAthlete subclass
public class AmateurAthlete : Athlete
{
    // properties
    public string WeightClass { get; set; }

    // Constructor
    public AmateurAthlete(string name, string weightClass) : base(name)
    {
        WeightClass = weightClass;
    }

    public override string GetRecord()
    {
        return $"{Name} is an amateur athlete in the {WeightClass} weight class with {Wins} wins and {Losses} losses.";
    }
}

// main program
public class App
{
// Main method
    public static async Task Main(string[] args)
    {
       var fighter = new Athlete("Canelo");
        fighter.RecordWin();
        fighter.RecordWin();
        fighter.RecordWin();
        fighter.RecordLoss();
        Console.WriteLine(fighter.GetRecord());

        var proFighter = new ProAthlete("Floyd");
        proFighter.RecordWin();
        proFighter.RecordLoss();
        Console.WriteLine(proFighter.GetRecord());

        var amateurFighter =  new AmateurAthlete("John", "Lightweight");
        amateurFighter.RecordWin();
        amateurFighter.RecordLoss();
        Console.WriteLine(amateurFighter.GetRecord());

        List<Athlete> athletes = new List<Athlete>
        {
            new Athlete("Canelo"),
            new ProAthlete("Floyd"),
            new AmateurAthlete("John", "Lightweight")
        };

        foreach (var athlete in athletes)
        {
            athlete.RecordWin();
            athlete.RecordLoss();
            Console.WriteLine(athlete.GetRecord());
        }

// Get a list of just the amateur athletes (hint: you'll need is or OfType<AmateurAthlete>())
        var listOfAmateurs = athletes.OfType<AmateurAthlete>().ToList();
        Console.WriteLine("List of Amateur Athletes:");
        foreach (var amateur in listOfAmateurs)
        {
            Console.WriteLine(amateur.GetRecord());
        }

// Get the average number of wins across all athletes (.Average())
        var averageWins = athletes.Average(a => a.Wins);
        Console.WriteLine($"Average number of wins across all athletes: {averageWins}");

// Check if any athlete has more than 2 losses (.Any())

        bool hasAthleteWithMoreThanTwoLosses = athletes.Any(a => a.Losses > 2);
        Console.WriteLine(hasAthleteWithMoreThanTwoLosses);  // true or false

// Simulate a fight asynchronously
        var fightResult = await SimulateFightAsync("Canelo");
        Console.WriteLine(fightResult);
        
    }

// Simulate a fight asynchronously method that takes an athlete's name and returns a string indicating the result of the fight after a delay
    public static async Task<string> SimulateFightAsync(string athleteName)
        {
            await Task.Delay(2000);
            return $"{athleteName} has won the fight!";
        }


}

