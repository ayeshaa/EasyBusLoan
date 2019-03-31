using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using TravelSystem.Helpers;
using TravelSystem.Models;

namespace TravelSystem.Controllers
{
    public class BlogController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? page)
        {
            int skip = 0;
            int take = 5;
            if (!page.HasValue)
            {
                page = 1;
            }
            else
            {
                if (page == 1)
                {
                    skip = 0;
                }
                else
                {
                    skip = (page.Value * take) - take;
                }
            }
            ViewBag.TotalBlogs = _context.Blogs.Count();
            var result = await _context.Blogs.Skip(skip).Take(take).ToListAsync();
            return View(result);
        }
        public IActionResult Detail(int id)
        {
            var blog = _context.Blogs.Include(o => o.BlogComments).ThenInclude(o => o.User).FirstOrDefault(o => o.Id == id);
            var _blog =  blog.BlogComments.Count;
           return View(blog);
        }
        public ActionResult BlogComment(BlogComments blog,int Id)
        {
            int userId = GetLoggedInUserId();
            if (userId > 0)
            {
                var _blog = _context.BlogComments.FirstOrDefault(o => o.BlogId == blog.BlogId && o.UserId == userId);
                if (_blog == null)
                {
                    blog.UserId = userId;
                    _context.BlogComments.Add(blog);
                    _context.SaveChanges();
                }
                return RedirectToAction("Detail", "Blog", new { id = blog.BlogId });
            }
           return RedirectToAction("LogIn", "User", new { blogId = blog.BlogId });
        }
    }
}