using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

    public record UpdatePostRequest
    (
        [Required]
        string Title,
        [Required]
        string Text
    );
