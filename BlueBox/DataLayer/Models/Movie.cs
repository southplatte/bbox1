using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BlueBox.DataLayer.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public string Synopsis { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Studio { get; set; }
       
    }

}
