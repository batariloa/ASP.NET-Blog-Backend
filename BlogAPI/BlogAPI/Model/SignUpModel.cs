using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


    public class SignUpModel
    {
        
        [Required]
        public String Firstname{get;set;}
        [Required]
        public String Lastname{get;set;}
        [Required]
        public String Username{get;set;}
        [Required]

        public String Email{get;set;}
        [Required]
        [Compare("ConfirmPassword")]
        public String Password{get;set;}   
        [Required]
        public String ConfirmPassword{get;set;}
    }
