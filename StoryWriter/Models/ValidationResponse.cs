using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryWriter.Models
{
    public class ValidationResponse
    {
        public bool Validated { get; set; }
        public List<string> ValidationMessages { get; set; }

        public ValidationResponse()
        {
            ValidationMessages = new List<string>();
        }
    }
}