namespace BoxingApp.Api.Models
{
    public class CreateAthleteDto
    {
        public string Name { get; set; }
        public string WeightClass { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
    }
}


// CreateAthleteDto (representing what a client is allowed to send)
// DTO stands for Data Transfer Object
//A DTO is just a class whose only job is to define exactly what data moves in or out of your API at a specific moment


//Where DTOs commonly show up
// Input DTOs (like your CreateAthleteDto) — controls exactly what a client can send you, blocking fields like Id that shouldn't be client-controlled
// Output DTOs — sometimes you don't want to send back everything in your database model either. 
// Imagine Athlete later had a PasswordHash or internal CreatedByAdminId field
//  — you'd want a separate DTO for responses too, 
//  so you never accidentally leak internal data to the client


// "Why not just use your entity classes directly in the API?"
//  is a genuinely common question. The honest answer: 
//  it works for small learning projects, 
//  but breaks down once your database model needs fields the client should never see or se