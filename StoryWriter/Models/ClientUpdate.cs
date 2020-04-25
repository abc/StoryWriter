using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryWriter.Models
{
    public class ClientUpdate
    {
        public Guid SenderId { get; set; }
        public string Fragment { get; set; }
    }
}