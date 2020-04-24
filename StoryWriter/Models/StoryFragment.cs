using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryWriter.Models
{
    public class StoryFragment
    {
        /// <summary>
        /// The text of the story fragment.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The writer who authored this element of the story.
        /// </summary>
        public Writer Author;

        /// <summary>
        /// Whether or not this fragment is intended to bring the story to a conclusion.
        /// </summary>
        public bool Ending;

        /// <summary>
        /// Constructor method for a story fragment.
        /// </summary>
        public StoryFragment()
        {
            Author = new Writer();
        }
    }
}