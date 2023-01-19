using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolControl.Data;
using SchoolControl.Models;

namespace SchoolControl.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public string gradeName = "";
        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
          


            var _grades = from g in _context.Grade
                         select g;

            var studentsGradeIDQuerry = (from s in _context.Student
                                         group s by new { s.StudentID, s.Grade.GradeId }
                                         into g
                                         select new
                                         {

                                             g.Key.GradeId,
                                             g.Key.StudentID


                                         }).ToList();


            foreach ( var s in _context.Student)
            {

                foreach (var q in studentsGradeIDQuerry)
                {

                    if (s.StudentID == q.StudentID )
                    {

                        foreach (var g in _grades)
                        {

                            if (q.GradeId == g.GradeId)
                            {

                                s.Grade = g;
                            }
                        }

                    }
                }
                

            }


            return View(await _context.Student.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.StudentID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {

            // Usa LINQ. para pegar todos os gradeName das turmas do banco grade 
            IQueryable<string> gradesQuerry = from p in _context.Grade
                                              orderby p.GradeName
                                              select p.GradeName;


            //joga os nomes das turmas no selectlist items
            SelectList items = new SelectList(await gradesQuerry.Distinct().ToListAsync());

            //forma uma viewbag que vai receber e mostrar no HTML
            ViewBag.gradesName = items;

         

            return View();


        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentID,StudentName,DateOfBirth,Photo,Height,Weight")] Student student, string gradeName )
        {



            var _grades = from g in _context.Grade
                          select g;


            foreach (var gr in _grades)
            {
                if (gr.GradeName == gradeName) student.Grade = gr;
               

            }


            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentID,StudentName,DateOfBirth,Photo,Height,Weight")] Student student)
        {
            if (id != student.StudentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.StudentID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Student.FindAsync(id);
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.StudentID == id);
        }
    }
}
