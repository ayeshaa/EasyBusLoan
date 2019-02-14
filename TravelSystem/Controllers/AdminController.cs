using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using TravelSystem.Models;

namespace TravelSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogIn(Admins admin)
        {
            var adminModel = await _context.Admins.FirstOrDefaultAsync(o => o.UserName == admin.UserName && o.Password == admin.Password);
            if (adminModel != null)
            {
                HttpContext.Session.SetInt32("Id", adminModel.Id);
                HttpContext.Session.SetString("UserName", adminModel.UserName);
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                TempData["Error"] = "Invalid Username or Password";
                return RedirectToAction("Index", "Admin");
            }
        }
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult AddBlog(int? id)
        {
            var blog = new Blogs();
            if (id.HasValue)
            {
                blog = _context.Blogs.FirstOrDefault(o => o.Id == id);
            }
            return View(blog);
        }
        public ActionResult SaveBlog(Blogs blogs)
        {
            var adminId = HttpContext.Session.GetInt32("Id");
            if (adminId.HasValue)
            {
                blogs.AdminId = adminId.Value;
                blogs.CreatedDate = blogs.Id > 0 ? blogs.CreatedDate : DateTime.Now;
                _context.Entry(blogs).State = blogs.Id > 0 ? EntityState.Modified : EntityState.Added;
                _context.SaveChanges();
                return RedirectToAction("Blogs");
            }
            else
            {
                return RedirectToAction("LogIn","Admin");
            }
        }
        public ActionResult Blogs()
        {
            return View();
        }
        public async Task<IActionResult> GetAllBlogs()
        {
            var result = await _context.Blogs.ToListAsync();
            return Json(result);
        }
        public ActionResult BlogCategories()
        {
            return View();
        }
        public ActionResult AddBlogCategories(int? id)
        {
            var blogCategory = new BlogCategories();
            if (id.HasValue)
            {
                blogCategory = _context.BlogCategories.FirstOrDefault(o => o.Id == id);
            }
            return View(blogCategory);
        }
        public async Task<IActionResult> GetAllBlogCategories()
        {
            var result = await _context.BlogCategories.ToListAsync();
            return Json(result);
        }
        public ActionResult SaveBlogCategories(BlogCategories blogCategories)
        {
            _context.Entry(blogCategories).State = blogCategories.Id > 0 ? EntityState.Modified : EntityState.Added;
            _context.SaveChanges();
            return RedirectToAction("BlogCategories");
        }
        public async Task<ActionResult> DeleteBlogCategory(int id)
        {
            var blogCategory = _context.BlogCategories.FirstOrDefault(o => o.Id == id);
            if (blogCategory != null)
            {
                _context.BlogCategories.Remove(blogCategory);
                await _context.SaveChangesAsync();
            }
            return Json(true);  
        }
        public async Task<ActionResult> DeleteBlog(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(o => o.Id == id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();
            }
            return Json(true);
        }
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");

        }
    }
}