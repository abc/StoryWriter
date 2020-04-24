using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryWriter.Models
{
    public class Writer
    {
        /// <summary>
        /// The name of the writer.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The writer's score so far.
        /// </summary>
        public int Score;

        /// <summary>
        /// A guid is needed to uniquely identify the user.
        /// </summary>
        public Guid Identifier { get; set; }
    }
}