using IdentityRefreshToken.Data;
using IdentityRefreshToken.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityRefreshToken.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class HouseController : ControllerBase
    {
        private readonly HouseDbContext _context;

        public HouseController(HouseDbContext context)
        {
            _context = context;
        }


        // GET: api/<HouseController>
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            var houses = _context.Houses.ToList();
            return Ok(houses);
        }

        // GET api/<HouseController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var house = _context.Houses.Where(x => x.ID == id);

            return Ok(house);
        }

        // POST api/<HouseController>
        [HttpPost]
        public IActionResult Post([FromBody] House HouseItem)
        {
            if (ModelState.IsValid)
            {
                _context.Houses.Add(HouseItem);
                _context.SaveChanges();
            }
            return Ok("New house Added");
        }

        // PUT api/<HouseController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] House HouseItem)
        {
            var existingHouse = _context.Houses.Where(x=> x.ID == id).FirstOrDefault();
            if (existingHouse == null)
                return NotFound();
            else
            {
                existingHouse.Address = HouseItem.Address;
                existingHouse.ConstructionYear = HouseItem.ConstructionYear;
                existingHouse.HouseName = HouseItem.HouseName;
                _context.SaveChanges();
            }
            return Ok("house updated");
        }

        // DELETE api/<HouseController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingHouse = _context.Houses.Where(x => x.ID == id).FirstOrDefault();
            if (existingHouse == null)
                return NotFound();
            else
            {
                _context.Houses.Remove(existingHouse);
                _context.SaveChanges();
            }
            return Ok("House Removed");
        }
    }
}
