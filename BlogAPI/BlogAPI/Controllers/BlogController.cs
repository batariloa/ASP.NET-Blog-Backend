using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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

        public PostController(ApplicationDbContext context){

            _context = context;

        }
        [HttpPost()]
        public IActionResult CreatePost([FromBody] CreatePostRequest postRequest){


        
            var username = User?.Identity?.Name;


            var dbUser = _context.Users.Where(b=> b.UserName == username).First();



            if(dbUser == null) return Unauthorized("You are not logged in.");

            Console.WriteLine(dbUser.Id);

                 Post post = new Post(){
                Text = postRequest.Text,
                Title = postRequest.Title,
                TimePosted = DateTime.Now,
                AuthorId = dbUser.Id,
                OwnerId = dbUser.Id,
                Owner = dbUser
          
        
            };

            
            if(dbUser.posts == null){
                dbUser.posts = new List<Post>();

            }

            _context.Posts.Add(post);
            dbUser.posts.Add(post);
        
            
            _context.SaveChanges();


            post.Owner = null;
            return Ok(post);
        }

                [HttpPost("repost/{id}")]
        public IActionResult Repost(int id){


            var post = _context.Posts.FirstOrDefault(p=> p.Id == id);

            Console.WriteLine("POST TEXT JE " + post.Text + "  A POST OWNERID JE " + post.OwnerId);

            var username = User?.Identity?.Name;

            var dbUser = _context.Users.Where(b=> b.UserName == username).Include(x=> x.posts).First();

            if(dbUser.posts == null){
                dbUser.posts = new List<Post>();

            }


            
            Post repost = new Post(){
                Id = 0,
                Text = post.Text,
                Title = post.Title,
                TimePosted = DateTime.Now,
                Repost = true,
                AuthorId = post.AuthorId,
                OwnerId = dbUser.Id

            };



            Console.WriteLine("dodajem korisniku "+ dbUser.UserName + " repost");
       
            dbUser.posts.Add(repost);

            _context.SaveChanges();


            

            return Ok("Reposted");
        }

                [HttpGet("{username}")]
        public IActionResult GetAllPostsUsername(string username){

            Console.WriteLine("USERNAME " + username);


            ApplicationUser dbUser = _context.Users.FirstOrDefault(b=> b.UserName == username);

            var user = _context.Users
                .Include(x=> x.posts.OrderByDescending(c => c.TimePosted))
                
                .Where(x => x.UserName == username)
                
                .FirstOrDefault();
               
            if(dbUser == null){
            
             return Ok(new List<Post>());
            }
            else {
                
                var posts = user.posts.Select(x=> new PostData(){
                    
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

        public string GetUsername(string id ){
        
            var user = _context.Users.Where(u=> u.Id == id).FirstOrDefault();

            string username = user.UserName;

            if(username == null) return " ";

            return username;


        }

        [HttpGet()]
        public IActionResult GetAllPosts(){

            var username = User?.Identity?.Name;

            Console.WriteLine("USERNAME " + username);


            ApplicationUser dbUser = _context.Users.FirstOrDefault(b=> b.UserName == username);

            var user = _context.Users
                .Include(x=> x.posts.OrderByDescending(c => c.TimePosted))
                
                .Where(x => x.UserName == username)
                
                .FirstOrDefault();
               
            if(dbUser == null){
            
             return Ok(new List<Post>());
            }
            else {

                var posts = user.posts.Select(x=> new PostData(){
                    
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
        public IActionResult UpdatePost(int id, UpdatePostRequest model){

            var post = _context.Posts.FirstOrDefault(p => p.Id == id);

            if(post == null) return NotFound("No post with such id.");

            var user = _context
                .Users
                .Include(x => x.posts)
                .Where(x=> x.posts.Any(p=>p.Id == id))
                .FirstOrDefault();


            var username = User?.Identity?.Name;

            if (username == user.UserName){
                
                post.Text = model.Text;
                post.Title = model.Title;

                
                _context.Posts.Update(post);
                _context.SaveChanges();

            } else {
                return Unauthorized("You are not the owner of the post.");
            }

            return Ok("Updated");
        }

                [HttpDelete("{id}")]
        public IActionResult DeletePost(int id){

                        Console.WriteLine("ID ID " + id);


            var post = _context.Posts.FirstOrDefault(p => p.Id == id);

            if(post == null) return NotFound("No post with such id.");

            var user = _context
                .Users
                .Include(x => x.posts)
                .Where(x=> x.posts.Any(p=>p.Id == id))
                .FirstOrDefault();


            var username = User?.Identity?.Name;

            if (username == user.UserName){
                
                user.posts.Remove(post);
                _context.Posts.Remove(post);
                _context.SaveChanges();

            } else {
                return Unauthorized("You are not the owner of the post.");
            }



            Console.WriteLine("Post" + user.Id);
            return Ok("Deleted");

        }
        
    }
