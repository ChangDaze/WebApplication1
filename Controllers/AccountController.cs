using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // GET: /Account/Register
        public IActionResult Register() => View();

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel form)
        {
            if (!ModelState.IsValid)
                return View(form);

            var user = new IdentityUser { UserName = form.Email, Email = form.Email };
            var result = await _userManager.CreateAsync(user, form.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("New account registered: {Email}", form.Email);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            _logger.LogWarning("Registration failed for {Email}: {Errors}",
                form.Email, string.Join("; ", result.Errors.Select(e => e.Description)));

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(form);
        }

        // GET: /Account/Login
        public IActionResult Login() => View();

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel form)
        {
            if (!ModelState.IsValid)
                return View(form);

            var result = await _signInManager.PasswordSignInAsync(
                form.Email, form.Password, form.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in: {Email}", form.Email);
                return RedirectToAction("Index", "Home");
            }

            _logger.LogWarning("Failed login attempt for {Email}", form.Email);
            ModelState.AddModelError(string.Empty, "Invalid email or password");
            return View(form);
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var email = User.Identity?.Name;
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out: {Email}", email ?? "(unknown)");
            return RedirectToAction("Index", "Home");
        }
    }
}
