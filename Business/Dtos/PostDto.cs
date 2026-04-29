using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos
{
    public class PostCreateDto
    {
        public int CustomerId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int Type { get; set; }
        public string Category { get; set; }
    }
}
