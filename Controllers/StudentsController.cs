using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST_wyklad.Models;

namespace REST_wyklad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;


        public StudentsController(StudentDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentModel>>> GetStudents()
        {
            try
            {
                return await _context.Students
                .Select(x => new StudentModel()
                {
                    StudentID = x.StudentID,
                    StudentImie = x.StudentImie,
                    StudentKierunek = x.StudentKierunek,
                    StudentNrIndeksu = x.StudentNrIndeksu,
                    ImageName = x.ImageName,
                    ImageSrc = String.Format("{0}://{1}{2}/Images/{3}", Request.Scheme, Request.Host, Request.PathBase, x.ImageName)
                })
                .ToListAsync();
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }

        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentModel>> GetStudentModel([FromRoute]int id)
        {
            try
            {
                var studentModel = await _context.Students
                    .Select(x => new StudentModel()
                    {
                        StudentID = x.StudentID,
                        StudentImie = x.StudentImie,
                        StudentKierunek = x.StudentKierunek,
                        StudentNrIndeksu = x.StudentNrIndeksu,
                        ImageName = x.ImageName,
                        ImageSrc = String.Format("{0}://{1}{2}/Images/{3}", Request.Scheme, Request.Host, Request.PathBase, x.ImageName)
                    })
                    .FirstOrDefaultAsync(i => i.StudentID == id);

                if (studentModel == null)
                {
                    //return NotFound();
                    return StatusCode(404, "No such student has been found");
                }

                return studentModel;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }

        }

        // PUT: api/Students/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudentModel(int id, [FromForm]StudentModel studentModel)
        {
            try
            {
                if (id != studentModel.StudentID)
                {
                    //return BadRequest();
                    return StatusCode(400, "No such student has been found");
                }

                if (studentModel.ImageFile != null)
                {
                    DeleteImage(studentModel.ImageName);
                    studentModel.ImageName = await SaveImage(studentModel.ImageFile);
                }

                _context.Entry(studentModel).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentModelExists(id))
                    {
                        //return NotFound();
                        return StatusCode(404, "No such student has been found");
                    }
                    else
                    {
                        throw;
                    }
                }

                //return NoContent();
                return StatusCode(204, "No content has been sent");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        // POST: api/Students
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StudentModel>> PostStudentModel([FromForm]StudentModel studentModel)
        {
            try
            {
                studentModel.ImageName =await SaveImage(studentModel.ImageFile);
                _context.Students.Add(studentModel);
                await _context.SaveChangesAsync();

                return StatusCode(201);
                //return CreatedAtAction("GetStudentModel", new { id = studentModel.StudentID }, studentModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentModel(int id)
        {
            try
            {
                var studentModel = await _context.Students.FindAsync(id);
                if (studentModel == null)
                {
                    return NotFound();
                }
                DeleteImage(studentModel.ImageName);
                _context.Students.Remove(studentModel);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        private bool StudentModelExists(int id)
        {
            return _context.Students.Any(e => e.StudentID == id);
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile)
        {
            string imageName=new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ','-');
            imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return imageName;
        }
        [NonAction]
        public void DeleteImage(string imageName)
        {
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }
    }
}
