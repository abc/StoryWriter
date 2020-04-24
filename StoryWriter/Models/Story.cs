using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryWriter.Models
{
    public class Story
    {
        /// <summary>
        /// The introduction to the story - the very first line.
        /// </summary>
        public StoryFragment Intro { get; set; }
        /// <summary>
        /// All of the story fragments so far.
        /// </summary>
        public List<StoryFragment> StoryFragments { get; set; }

        /// <summary>
        /// Constructor method for Story.
        /// </summary>
        public Story()
        {
            StoryFragments = new List<StoryFragment>();
            Intro = new StoryFragment();
        }
    }
}