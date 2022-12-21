using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

    public class SignInModel
    {
          [Required]
        public String Email{get;set;}
          [Required]
        public String Password{get;set;}   
    }
