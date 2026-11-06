// this is like import in JS (using)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BoxingApp.Api.Data;
using BoxingApp.Api.Models;

// this is like export in JS (namespace)
namespace BoxingApp.Api.Controllers 
{
    [ApiController]
    // controller is placholder for the name of the controller, in this case AthletesController and it will be api/athletes
    [Route("api/[controller]")]
    public class AthletesController : ControllerBase 
    {
        // this is a private field that will hold the context of the database, which is used to access the database
        private readonly BoxingAppContext _context;
        public AthletesController(BoxingAppContext context) 
        {
            _context = context;
        }

        // GET Requests (READ) - these are the endpoints that will be used to get data from the database
        // GET request to api/athletes will return the list of athletes
        [HttpGet]
        public async Task<IActionResult> GetAthletes() 
        {
            var athletes = await _context.Athletes.ToListAsync();
            return Ok(athletes);
        }

        // GET request to api/athletes/{id} will return the athlete at the given index
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAthleteById (int id) 
        {
            var athlete = await _context.Athletes.FindAsync(id);
            if (athlete == null) 
            {
                return NotFound();
            }
            return Ok(athlete);
        }

        // POST Requests (CREATE) - these are the endpoints that will be used to create data in the database
        // POST request to api/athletes will create a new athlete
        [HttpPost]
        public async Task<IActionResult> CreateAthlete(CreateAthleteDto dto) 
        {
            // this will create a new athlete object from the dto and add it to the database
            var athlete = new Athlete
            {
                Name = dto.Name,
                WeightClass = dto.WeightClass,
                Wins = dto.Wins,
                Losses = dto.Losses
            };

            // this will add the athlete to the database and save the changes
            _context.Athletes.Add(athlete);
            await _context.SaveChangesAsync();
            // this will return a 201 Created response with the location of the new athlete
            return CreatedAtAction(nameof(GetAthleteById), new { id = athlete.Id }, athlete);
        }

        // PUT Requests (UPDATE) - these are the endpoints that will be used to update data in the database
        // PUT request to api/athletes/{id} will update the athlete at the given index
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAthlete(int id, Athlete updatedAthlete) 
        {
            var athlete = await _context.Athletes.FindAsync(id);
            if (athlete == null) 
            {
                return NotFound();
            }

            athlete.Name = updatedAthlete.Name;
            athlete.WeightClass = updatedAthlete.WeightClass;
            athlete.Wins = updatedAthlete.Wins;
            athlete.Losses = updatedAthlete.Losses;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE Requests (DELETE) - these are the endpoints that will be used to delete data in the database
        // DELETE request to api/athletes/{id} will delete the athlete at the given index
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAthlete(int id)
        {
            var athlete = await _context.Athletes.FindAsync(id);
            if (athlete == null)
            {
                return NotFound();
            }

            _context.Athletes.Remove(athlete);
            await _context.SaveChangesAsync();
            return NoContent();
        }


    }   


}