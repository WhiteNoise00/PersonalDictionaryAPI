using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalDictionaryAPI.Data;
using PersonalDictionaryAPI.Models;

namespace PersonalDictionaryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguageController : Controller
    {
        private readonly APIDbContext _db;

        public LanguageController(APIDbContext context)
        {
            _db = context;
        }

        [HttpPost]
        public ActionResult CreateLanguage(Language lg)
        {
            try
            {
                _db.Languages.Add(lg);
                _db.SaveChanges();
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public ActionResult GetLanguage(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            else
            {
                Language language = _db.Languages.Find(id);
                if (language is null)
                {
                    return NotFound();
                }
                return Ok(language);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAllLanguages()
        {
            try
            {
                List<Language> languages = await _db.Languages.ToListAsync();
                return Ok(languages);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        
        [HttpPut("{id}")]
        public ActionResult EditLanguage(int? id, Language lg)
        {
            if (id is null)
            {
                return BadRequest();
            }
            else
            {
                Language language = _db.Languages.Find(id);
                if (language is null)
                {
                    return NotFound();
                }

                language.Name = lg.Name;
                language.Country = lg.Country;
                _db.Languages.Update(language);
                _db.SaveChanges();
                return Ok();
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteLanguage(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            else
            {
                try
                {
                    Language language = _db.Languages.Find(id);
                    if (language is null)
                    {
                        return NotFound();

                    }
                    else
                    {
                        _db.Languages.Remove(language);
                        _db.SaveChanges();
                        return Ok();
                    }
                }
                catch (Exception)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }        
    }
}
