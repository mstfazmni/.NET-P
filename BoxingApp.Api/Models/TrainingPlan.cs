namespace BoxingApp.Api.Models
{
    public class TrainingPlan
    {
        public int Id { get; set; }
        public int AthleteId { get; set; }
        public string PlanName { get; set; }
        public Athlete Athlete { get; set; }  // navigation property — explained below
    }
}

// The Athlete property is a navigation property that allows you to access the related Athlete entity for a given TrainingPlan. 
// This is useful for querying and working with related data in Entity Framework Core.