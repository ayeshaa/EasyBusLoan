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
        public async Task<IActionResult> Index()
        {
            var blogs = await _context.Blogs.Include(o=>o.Admins).ToListAsync();
            blogs.ForEach(e =>
            {
                List<string> categoryNames = new List<string>();
                var categoryIds = e.CategoryIds.Split(",");
                foreach (var categoryId in categoryIds)
                {
                    var categoryName = _context.BlogCategories.Where(o => o.Id == Convert.ToInt32(categoryId)).Select(o => o.CategoryName).FirstOrDefault();
                    categoryNames.Add(categoryName);
                }
                e.Body = Html.RemoveHtml(e.Body);
                e.CategoryIds = String.Join(",", categoryNames);
            });
            return View(blogs);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var blog = await _context.Blogs.Where(o => o.Id==id).FirstOrDefaultAsync();
            return View(blog);
        }
    }
}