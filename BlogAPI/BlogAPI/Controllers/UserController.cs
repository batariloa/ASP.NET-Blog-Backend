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
using BlogAPI;

namespace BlogAPI.Controllers
{
    
    [ApiController]
    [Route("auth")]
    public class UserController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
    public UserController(IConfiguration configuration, UserManager<ApplicationUser> userManager){
        
        _userManager = userManager;
        _configuration = configuration;
    }


    [HttpPost("login")]
    public async Task<IActionResult>  UserLogin([FromBody] SignInModel model){


        ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);


        if(user!= null && await _userManager.CheckPasswordAsync(user, model.Password)){

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>{

                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach(var userRole in userRoles){
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }


            var token = GetToken(authClaims);
            return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    username = user.UserName,
                    id = user.Id
                });
        }

    return Unauthorized();


    }

    [HttpPost("register")]
    public async Task<IActionResult>  UserRegister([FromBody] SignUpModel model){


        var emailExists = await _userManager.FindByEmailAsync(model.Email);
        var usernameExists = await _userManager.FindByNameAsync(model.Username);

        if(emailExists != null || usernameExists !=null){ 
                         Console.WriteLine("ZAZA");

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        ApplicationUser user = new ApplicationUser(){
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username,
            posts = new List<Post>()
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if(!result.Succeeded){
                         Console.WriteLine("ZAZA2");


            string msg = "Errors: ";
            foreach(var error in result.Errors){
            
            Console.WriteLine("Errr " + error.Description);
            msg.Concat("\n"+error.Description);
            }
           return StatusCode(StatusCodes.Status500InternalServerError, msg);

        }

    

        return Ok("Registered");
       
    }


       private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:SecretKey")));

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