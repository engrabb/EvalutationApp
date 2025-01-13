using EvalAppBackEnd.Data;
using EvalAppBackEnd.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EvalAppBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet("data")]
        public IActionResult GetData()
        {
            var result = new
            {
                Message = "Data fetched successfully",
                Success = true
            };

            return Ok(result);
        }


        // Register User - Improved Version
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid model");

            // Check if the user already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest("User with this email already exists.");

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(new { message = "Error creating user", errors = result.Errors });

            var roleExist = await _roleManager.RoleExistsAsync("Player");
            if (!roleExist)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole("Player"));
                if (!roleResult.Succeeded)
                    return BadRequest(new { message = "Error creating role" });
            }

            // Assign role to the user (e.g., Player)
            await _userManager.AddToRoleAsync(user, "Player");

            return Ok(new { message = "Registration successful!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid model");

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                return Ok(new { message = "Login successful!" });
            }

            return Unauthorized(new { message = "Invalid credentials" });
        }
    }
}

