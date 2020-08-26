using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storyteller.Models
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
        public int Score { get; set; }

        /// <summary>
        /// A guid is needed to uniquely identify the user.
        /// </summary>
        public string Identifier { get; set; }

        public PlayerColor Color { get; set; }
    }
}
