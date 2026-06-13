using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            // 1. Create default data
            var userProfile = new UserProfile
            {
                Name = "Jane Doe",
                Email = "jane.doe@example.com",
                JobTitle = "Full Stack Developer",
                Bio = "I am a passionate developer who loves building clean, scalable web applications using .NET and modern frontend tools."
            };

            // 2. Pass the data to the View
            return View(userProfile);
        }
    }
}