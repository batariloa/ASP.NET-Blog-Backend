using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


    public record CreatePostRequest
    (
        [Required(ErrorMessage ="Please provide the post title.")]
        string Title,
        [Required(ErrorMessage ="Please provide the post's body.")]
        string Text
    );
