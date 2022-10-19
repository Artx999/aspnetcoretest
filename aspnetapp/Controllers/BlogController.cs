using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using assignment_4.Data;
using assignment_4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace assignment_4.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _um;

        public BlogController(ApplicationDbContext context, UserManager<ApplicationUser> um)
        {
            _context = context;
            _um = um;
        }

        // GET: Blog
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Blogs.Include(b => b.ApplicationUser).OrderByDescending(a => a.Timestamp);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Blog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blog/Create
        [Authorize]
        public IActionResult Add()
        {
            ViewData["ApplicationUserId"] = new SelectList(_um.Users, "Id", "Nickname");
            return View();
        }

        // POST: Blog/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Add([Bind("Title,Summary,Content")] Blog blog)
        {
            // Check if user is logged in and link it to the post
            if (User.Identity.IsAuthenticated)
            {
                blog.ApplicationUser = _um.GetUserAsync(User).Result;
            }
            
            ModelState.Clear();
            TryValidateModel(blog);
            
            if (ModelState.IsValid)
            {
                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_um.Users, "Id", "Nickname", blog.ApplicationUserId);
            return View(blog);
        }

        // GET: Blog/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            if (blog.ApplicationUserId != Guid.Parse(_um.GetUserId(User)))
            {
                return Forbid();
            }
            
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Nickname", blog.ApplicationUserId);
            return View(blog);
        }

        // POST: Blog/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Summary,Content")] Blog blog)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            // This way of getting th ApplicationUser and updating the blog post might not be good
            var updatedBlog = _context.Blogs
                .Include(blog => blog.ApplicationUser)
                .FirstOrDefault(x => x.Id == id);

            if (updatedBlog == null)
            {
                return NotFound();
            }

            updatedBlog.Title = blog.Title;
            updatedBlog.Summary = blog.Summary;
            updatedBlog.Content = blog.Content;

            ModelState.Clear();
            TryValidateModel(updatedBlog);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(updatedBlog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(updatedBlog.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Nickname", blog.ApplicationUserId);
            return View(blog);
        }

        // GET: Blog/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Blogs == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Blogs'  is null.");
            }
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
          return (_context.Blogs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
