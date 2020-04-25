using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryWriter.Models
{
    public class ServerUpdate
    {
        public int SecondsToVote { get; set; }
        public List<StoryFragment> FragmentsThisRound { get; set; }
        public List<int> Votes { get; set; }
        public List<Writer> WritersPresent { get; set; }
        public bool TimeToVote { get; set; }
        public bool VotesChanged { get; set; }
    }
}