using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BlogAPI.Controllers;


[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("post")]
public class PostController : ControllerBase
{

    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;


    public PostController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
    }


    //Method to get username of user by id, used only in this controller
    public string GetUsername(string id)
    {
        var user = _context.Users.Where(u => u.Id == id).FirstOrDefault();
        string username = user.UserName;

        if (username == null) return " ";

        return username;
    }

    //Endpoints

    [HttpPost()]
    public IActionResult CreatePost([FromBody] CreatePostRequest postRequest)
    {

        var username = User?.Identity?.Name;
        var dbUser = _context.Users.Where(b => b.UserName == username).First();

        if (dbUser == null) return Unauthorized("You are not logged in.");

        Post post = new Post()
        {
            Text = postRequest.Text,
            Title = postRequest.Title,
            TimePosted = DateTime.Now,
            AuthorId = dbUser.Id,
            OwnerId = dbUser.Id,
            Owner = dbUser
        };


        if (dbUser.posts == null) { dbUser.posts = new List<Post>(); }

        _context.Posts.Add(post);
        dbUser.posts.Add(post);
        _context.SaveChanges();


        var postResponse = new PostResponse()
        {
            AuthorUsername = GetUsername(post.AuthorId),
            Text = post.Text,
            Title = post.Title,
            OwnerUsername = post.Owner.UserName,
            OwnerId = post.OwnerId
        };
        return Ok(postResponse);
    }

    [HttpPost("repost/{id}")]
    public IActionResult Repost(int id)
    {

        var post = _context.Posts.FirstOrDefault(p => p.Id == id);

        if (post == null) return BadRequest("That post doesn't exist.");

        var username = User?.Identity?.Name;

        var dbUser = _context.Users.Where(b => b.UserName == username).Include(x => x.posts).First();

        if (dbUser.posts == null) { dbUser.posts = new List<Post>(); }

        Post repost = new Post()
        {
            Id = 0,
            Text = post.Text,
            Title = post.Title,
            TimePosted = DateTime.Now,
            Repost = true,
            AuthorId = post.AuthorId,
            OwnerId = dbUser.Id
        };

        dbUser.posts.Add(repost);
        _context.SaveChanges();

        return Ok("Reposted");
    }

    [HttpGet("{username}")]
    public IActionResult GetAllPostsUsername(string username)
    {

        var user = _context.Users
            .Include(x => x.posts!.OrderByDescending(c => c.TimePosted))
            .Where(x => x.UserName == username)
            .FirstOrDefault();

        if (user == null) return NotFound("No such user");

        if (user.LockoutEnd != null) return NotFound("User has been suspended");


        var posts = user.posts!.Select(x => new PostResponse()
        {

            Id = x.Id,
            AuthorUsername = GetUsername(x.AuthorId ?? ""),
            Text = x.Text,
            Title = x.Title,
            OwnerId = x.OwnerId,
            AuthorId = x.AuthorId,
            OwnerUsername = x.Owner!.UserName,
            Repost = x.Repost


        });

        return Ok(posts);



    }


    [HttpGet()]
    public IActionResult GetAllPosts()
    {

        var username = User?.Identity?.Name;

        var user = _context.Users
            .Include(x => x.posts.OrderByDescending(c => c.TimePosted))
            .Where(x => x.UserName == username)
            .FirstOrDefault();

        if (user == null) return Unauthorized("User not found.");
        else
        {

            var posts = user.posts.Select(x => new PostResponse()
            {

                Id = x.Id,
                AuthorUsername = GetUsername(x.AuthorId),
                Text = x.Text,
                Title = x.Title,
                OwnerId = x.OwnerId,
                AuthorId = x.AuthorId,
                OwnerUsername = x.Owner.UserName,
                Repost = x.Repost



            });
            return Ok(posts);
        }

    }

    [HttpPut("{id}")]
    public IActionResult UpdatePost(int id, UpdatePostRequest model)
    {

        var post = _context.Posts.FirstOrDefault(p => p.Id == id);

        if (post == null) return NotFound("No post with such id.");

        var user = _context
            .Users
            .Include(x => x.posts)
            .Where(x => x.posts!.Any(p => p.Id == id))
            .FirstOrDefault();


        if (user == null) return Unauthorized("User not found.");

        var username = User?.Identity?.Name;

        if (username == user.UserName)
        {

            post.Text = model.Text;
            post.Title = model.Title;


            _context.Posts.Update(post);
            _context.SaveChanges();

        }
        else
        {
            return Unauthorized("You are not the owner of the post.");
        }

        return Ok(new MessageResponse() { Message = "Updated" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {

        var post = await _context.Posts.Include(x => x.Owner).FirstAsync(p => p.Id == id);

        if (post == null) return NotFound("No post with such id.");

        var isAdmin = HttpContext.User.IsInRole("Admin");


        var username = User?.Identity?.Name;

        if (username == post.Owner!.UserName || isAdmin)
        {

            _context.Posts.Remove(post);
            _context.SaveChanges();

        }
        else
        {
            return Unauthorized("You are not the owner of the post.");
        }


        return Ok(new MessageResponse() { Message = "Deleted" });

    }



}
