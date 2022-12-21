using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Services
{
    public class BlogService : IBlogService
    {
        public void CreatePost(Post post){

        }

        public Post GetPost(Guid id){

            return new Post();
        }

        public List<Post> GetPostsForUsername(String username){



        return new List<Post>();
        }
        
    }
}