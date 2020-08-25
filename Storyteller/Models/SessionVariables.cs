using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storyteller
{
    /// <summary>
    /// This class stores the names for session variables - which are used to keep a track of pieces of data relating to a user's session, such as user ID. 
    /// </summary>
    public static class SessionVariables
    {
        /// <summary>
        /// The variable name for a user's ID
        /// </summary>
        public const string UserId = "UserId";
        /// <summary>
        /// The variable name for messages such as alerts or warnings.
        /// </summary>
        public const string Messages = "Messages";
        /// <summary>
        /// The variable name for the user's room code.
        /// </summary>
        public const string RoomCode = "RoomCode";
    }
}
