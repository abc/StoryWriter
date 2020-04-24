using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryWriter.Models
{
    public class Room
    {
        /// <summary>
        /// The invite code which is typed in to join the room.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The user-friendly display name of the room
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The writer who created the room
        /// </summary>
        public Writer Owner { get; set; }

        /// <summary>
        /// The list of writers who are taking part in the story.
        /// </summary>
        public List<Writer> Writers { get; set; }

        /// <summary>
        /// The all-important story itself.
        /// </summary>
        public Story Story { get; set; }

        /// <summary>
        /// Constructor method for the room.
        /// </summary>
        public Room()
        {
            Writers = new List<Writer>();
            Story = new Story();
        }
    }
}