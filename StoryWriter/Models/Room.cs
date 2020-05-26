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

        public List<PlayerColor> ColorsInUse { get; set; }

        public List<Writer> PresentWriters { get; set; } 
        public List<Writer> AbsentWriters { get; set; }

        /// <summary>
        /// The all-important story itself.
        /// </summary>
        public Story Story { get; set; }

        public Dictionary<Guid, Guid> FragmentVotes { get; set; }

        /// <summary>
        /// Constructor method for the room.
        /// </summary>
        public Room()
        {
            // Writers = new List<Writer>();
            PresentWriters = new List<Writer>();
            AbsentWriters = new List<Writer>();
            Story = new Story();
            StartTime = DateTime.Now;
            FrameFragments = new List<StoryFragment>();
            FragmentVotes = new Dictionary<Guid, Guid>();
            LastUpdate = DateTime.Now;
            NextActionTime = DateTime.Now.AddSeconds(60);
            NextAction = ActionType.Vote;
            StartTime = DateTime.Now;
            ColorsInUse = new List<PlayerColor>();
        }

        public DateTime LastUpdate { get; set; }
        public DateTime NextActionTime { get; set; }
        public ActionType NextAction { get; set; }
        public DateTime StartTime { get; set; }
        public List<StoryFragment> FrameFragments { get; set; }
    }
}