using EvalAppBackEnd.Data;
using EvalAppBackEnd.Models;
using EvalAppBackEnd.Security;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IJwtService _jwtService;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
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


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid model");

            // Check if the user already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest("User with this email already exists.");

            
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { message = "Error creating user", errors = errorMessage });
            }

            if (!await _roleManager.RoleExistsAsync("Player"))
            {
                var playerRoleResult = await _roleManager.CreateAsync(new IdentityRole("Player"));
                if (!playerRoleResult.Succeeded)
                    return BadRequest(new { message = "Error creating Player role", errors = playerRoleResult.Errors });
            }

            // Assign the "Player" role to the user
            await _userManager.AddToRoleAsync(user, "Player");

            if (model.IsCoach)
            {
                if (!await _roleManager.RoleExistsAsync("Coach"))
                {
                    var coachRoleResult = await _roleManager.CreateAsync(new IdentityRole("Coach"));
                    if (!coachRoleResult.Succeeded)
                        return BadRequest(new { message = "Error creating Coach role", errors = coachRoleResult.Errors });
                }

                // Assign the "Coach" role to the user
                await _userManager.AddToRoleAsync(user, "Coach");
            }

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
                // Generate JWT token
                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtService.GenerateToken(user, roles.ToList()); // This assumes you have an IJwtService that generates the token

                return Ok(new
                {
                    message = "Login successful!",
                    token = token, // Return the token
                    user = new
                    {
                        email = user.Email,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                    }
                });
            }

            return Unauthorized(new { message = "Invalid credentials" });
        }

        [HttpGet("verify")]
        [Authorize] 
        public async Task<IActionResult> VerifyToken()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            return Ok(new
            {
                user = new
                {
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    roles = await _userManager.GetRolesAsync(user)
                }
            });
        }
    }
}

