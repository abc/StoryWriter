using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storyteller.Models
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
        public Writer Author { get; set; }

        /// <summary>
        /// Whether or not this fragment is intended to bring the story to a conclusion.
        /// </summary>
        public bool Ending { get; set; }

        public string Identifier { get; set; }

        /// <summary>
        /// Constructor method for a story fragment.
        /// </summary>
        public StoryFragment()
        {
            Author = new Writer();
            Identifier = Guid.NewGuid().ToString();
        }
    }
}
