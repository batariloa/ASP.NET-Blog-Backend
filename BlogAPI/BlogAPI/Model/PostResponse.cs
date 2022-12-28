using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class PostResponse
{
    public string? AuthorId { get; set; }
    public string? OwnerId { get; set; }
    public string? Text { get; set; }
    public string? Title { get; set; }
    public int Id { get; set; }
    public string? AuthorUsername { get; set; }

    public string? OwnerUsername { get; set; }

    public bool Repost { get; set; }

}
