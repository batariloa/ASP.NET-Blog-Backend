using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Services;
    public interface IBlogService
    {
        
        public void CreatePost(Post post);

        public Post GetPost(Guid id);

        public List<Post> GetPostsForUsername(String username);
    }
