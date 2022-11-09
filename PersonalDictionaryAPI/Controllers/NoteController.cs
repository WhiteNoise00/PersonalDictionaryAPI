
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonalDictionaryAPI.Data;
using PersonalDictionaryAPI.Models;

namespace PersonalDictionaryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : Controller
    {
        private readonly APIDbContext _db;

        public NoteController(APIDbContext context)
        {
            _db = context;
        }

        [HttpPost]
        public ActionResult CreateNote(Note note)
        {
            try
            {
                _db.Notes.Add(note);
                _db.SaveChanges();
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public ActionResult GetNote(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            else
            {
                Note note = _db.Notes.Find(id);
                if (note is null)
                {
                    return NotFound();
                }
                return Ok(note);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAllNotes()
        {
            try
            {
                List<Note> notes = await _db.Notes.ToListAsync();
                return Ok(notes);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }      

        [HttpPut("{id}")]
        public ActionResult EditNote(int? id, Note nt)
        {
            if (id is null)
            {
                return BadRequest();
            }
            else
            {
                Note note = _db.Notes.Find(id);
                if (note is null)
                {
                    return NotFound();
                }
                note.Text = nt.Text;
                note.Translation = nt.Translation;
                note.CategoryId = nt.CategoryId;
                note.Description = nt.Description;
                note.ImagePath = nt.ImagePath;
                _db.Notes.Update(note);
                _db.SaveChanges();
                return Ok();
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteNote(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            else
            {
                try
                {
                    Note note = _db.Notes.Find(id);
                    if (note is null)
                    {
                        return NotFound();

                    }
                    else
                    {
                        _db.Notes.Remove(note);
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
