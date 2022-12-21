using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id{get;set;}
        public String Title {get; set;}
        public String Text {get;set;}
   

            
        public string AuthorId {get;set;}
        public string OwnerId {get;set;}
        [ForeignKey("OwnerId")]
        public virtual ApplicationUser Owner{get;set;}
        public DateTime TimePosted {get;set;}

        public bool Repost {get;set;}



 

    }
