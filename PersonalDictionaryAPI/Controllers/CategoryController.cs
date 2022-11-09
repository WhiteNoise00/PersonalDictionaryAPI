using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalDictionaryAPI.Data;
using PersonalDictionaryAPI.Models;


namespace PersonalDictionaryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly APIDbContext _db;

        public CategoryController(APIDbContext context)
        {
            _db = context;
        }

        [HttpPost]
        public ActionResult CreateCategory(Category ct)
        {
            try
            {
                _db.Categories.Add(ct);
                _db.SaveChanges();
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public ActionResult GetCategory(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            else
            {
                Category category = _db.Categories.Find(id);
                if (category is null)
                {
                    return NotFound();
                }
                return Ok(category);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAllCatigories()
        {
            try
            {
                List<Category> categories = await _db.Categories.ToListAsync();
                return Ok(categories);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        } 

        [HttpPut("{id}")]
        public ActionResult EditCategory(int? id, Category ct)
        {
            if (id is null)
            {
                return BadRequest();
            }
            else
            {
                Category category = _db.Categories.Find(id);
                if (category is null)
                {
                    return NotFound();
                }

                category.Name = ct.Name;
                category.Description = ct.Description;
                _db.Categories.Update(category);
                _db.SaveChanges();
                return Ok();
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCategory(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            else
            {
                try
                {
                    Category category = _db.Categories.Find(id);
                    if (category is null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        _db.Categories.Remove(category);
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
