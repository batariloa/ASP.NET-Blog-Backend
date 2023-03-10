using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using BlogAPI;



namespace BlogAPI.Controllers
{

    [ApiController]
    [Route("auth")]
    public class UserController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        private readonly RoleManager<IdentityRole> _roleManager;



        public UserController(IConfiguration configuration, UserManager<ApplicationUser> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)

        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }


        [HttpPost("login")]
        public async Task<IActionResult> UserLogin([FromBody] SignInModel model)
        {

            //Get user by email
            var user = await _userManager.FindByEmailAsync(model.Email);


            if (user == null) return Unauthorized("Incorrect credentials");

            if (user.LockoutEnd != null) return Unauthorized("User suspended.");


            //Check that user exists, check that password matches
            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {

                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>{

                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }


                var token = GenerateToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    username = user.UserName,
                    id = user.Id,
                    roles = userRoles

                });
            }

            return Unauthorized("Incorrect credentials");

        }



        [HttpPost("register")]
        public async Task<IActionResult> UserRegister([FromBody] SignUpModel model)
        {

            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            var usernameExists = await _userManager.FindByNameAsync(model.Username);

            if (emailExists != null) return BadRequest("Email already exists.");

            if (usernameExists != null) return BadRequest("Username already taken.");


            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                posts = new List<Post>()
            };

            var claim = new Claim(ClaimTypes.Role, "User");




            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {

                string msg = "Errors: ";
                foreach (var error in result.Errors)
                {
                    msg.Concat("\n" + error.Description);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, msg);

            }


            if (!await _roleManager.RoleExistsAsync("User"))
            {

                await _roleManager.CreateAsync(new IdentityRole("User"));

            }
            await _userManager.AddToRoleAsync(user, "User");



            return Ok("Registered");

        }

        [HttpPut("ban/{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BanUser(string username)
        {

            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return BadRequest("No user with such username");

            var lockoutEndDate = new DateTime(2999, 01, 01);
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, lockoutEndDate);

            return Ok("User suspended");

        }


        [HttpPost("search/{term}")]
        public async Task<IActionResult> SearchUser(string term)
        {

            var results = _context.Users.Where(x => x.UserName.Contains(term)).Take(6);

            var userResults = await results.Select(x => new SearchUserData()
            {
                Username = x.UserName
            }).ToListAsync();


            return Ok(userResults);

        }

        //Generate new JWT token
        private JwtSecurityToken GenerateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:SecretKey") ?? ""));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }



}