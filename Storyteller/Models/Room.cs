using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storyteller.Models
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

        public bool Started { get; set; }

        public void Start()
        {
            Started = true;
            NextActionTime = DateTime.Now.AddSeconds(60);
            StartTime = DateTime.Now;
        }

        /// <summary>
        /// The all-important story itself.
        /// </summary>
        public Story Story { get; set; }

        public Dictionary<string, string> FragmentVotes { get; set; }

        /// <summary>
        /// Constructor method for the room.
        /// </summary>
        public Room()
        {
            // Writers = new List<Writer>();
            PresentWriters = new List<Writer>();
            AbsentWriters = new List<Writer>();
            Story = new Story();
            FrameFragments = new List<StoryFragment>();
            FragmentVotes = new Dictionary<string, string>();
            LastUpdate = DateTime.Now;
            NextAction = ActionType.Vote;
            ColorsInUse = new List<PlayerColor>();
        }

        public DateTime LastUpdate { get; set; }
        public DateTime NextActionTime { get; set; }
        public ActionType NextAction { get; set; }
        public DateTime StartTime { get; set; }
        public List<StoryFragment> FrameFragments { get; set; }
        public int MillisecondsToAction { get { return (int)Math.Round((NextActionTime - DateTime.Now).TotalMilliseconds); } }
    }
}
