using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AliceWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AliceWebApp.Controllers
{
    [Route("api/[controller]")]
    public class CharactersController : Controller
    {
        private IAliceRepository _rep;
        //private int idcounter = 1;
        //private List<Character> Characters = new List<Character>() {
        //    new Character { Id = 0, Name = "Alice" },
        //    new Character {Id = 1, Name = "Cheshire Cat" }
        //};

        public CharactersController(IAliceRepository repository)
        {
            _rep = repository;
        }


        // GET api/characters
        [HttpGet]
        public IEnumerable<Character> Get()
        {
            return _rep.GetAll() ;
        }

        // GET api/characters/5
        [HttpGet("{id}", Name ="GetById")]
        [Authorize(ActiveAuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult Get(int id)
        {
            var character = _rep.Find(id);
            if (character == null)
                return NotFound("Character not found");
            else
                return new ObjectResult(character);

        }

        // POST api/characters
        /// <returns>New Created Todo Item</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        [Authorize(ActiveAuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("create")]
        [ProducesResponseType(typeof(Character), 201)]
        [ProducesResponseType(typeof(Character), 400)]
        public IActionResult Create([FromBody]Character value)
        {
            if (value == null)
                return BadRequest();

            _rep.Add(value);

            return CreatedAtRoute("GetById", new { id = value.Id }, value);

        }

        // PUT api/characters/5
        [HttpPut("update/{id}")]
        [Authorize(ActiveAuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Update(int id, [FromBody] Character value)
        {
            if(value == null || value.Id != id)
            {
                return BadRequest();
            }

            var character = _rep.Find(id);
            if (character == null)
                return NotFound("Cannot found the character");

            character.Name = value.Name;
            _rep.Update(character);

            return CreatedAtRoute("GetById", new { id = value.Id }, value);

        }

        // DELETE api/characters/5
        [HttpDelete("delete/{id}")]
        [Authorize(ActiveAuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Delete(int id)
        {
            var character = _rep.Find(id);
            if (character == null)
                return NotFound("Cannot found the character");

            _rep.Remove(id);
            return Ok("Successfully removed");
        }
    }
}
