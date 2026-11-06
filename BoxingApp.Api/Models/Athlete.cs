namespace BoxingApp.Api.Models
{
    public class Athlete
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WeightClass { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
    }
}

// Athlete (representing what is stored in the database)