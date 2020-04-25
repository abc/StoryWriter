using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StoryWriter.Models;
using System.Text;

namespace StoryWriter.Service
{
    public static class RoomService
    {
        /// <summary>
        /// Generate a four-character code used to join a session in progress.
        /// </summary>
        /// <returns></returns>
        public static string GenerateCode()
        {
            var random = new System.Random();

            const string pool = "abcdefghijklmnopqrstuvwxyz";
            var builder = new StringBuilder();

            for (var i = 0; i < 4; i++)
            {
                var c = pool[random.Next(0, pool.Length)];
                builder.Append(c);
            }

            return builder.ToString().ToUpperInvariant();
        }

        public static Room Create (string roomName, Writer owner)
        {
            return new Room { Code = GenerateCode(), Name = roomName, Owner = owner };
        }

        public static DateTime NextFrame (Room room)
        {
            return room.StartTime.AddMinutes((room.FrameNumber * 1) + 1);
        }

        public static void RegisterFragment (Room room, Writer writer, string fragment)
        {
            // Find if the writer has an existing fragment.
            var existingFragmentForAuthor = room.FrameFragments.Where(w => w.Author.Identifier == writer.Identifier);

            // Update the writer's existing fragment, if it exists.
            if (existingFragmentForAuthor.Any())
            {
                existingFragmentForAuthor.Single().Text = fragment;
            }

            // If one doesn't exist, then add a new fragment.
            else
            {
                room.FrameFragments.Add(new StoryFragment { Author = writer, Text = fragment, Identifier = Guid.NewGuid() });
            }
        }

        public static void RegisterVote(Room room, Writer writer, string fragmentId)
        {
            room.FragmentVotes[writer.Identifier] = new Guid(fragmentId);
        }

        public static ServerUpdate GetUpdate (Room room)
        {
            var nextFrame = NextFrame(room);

            var secondsToVote = (DateTime.Now - nextFrame).Seconds;
            var timeToVote = nextFrame <= room.StartTime;
            var fragmentsThisRound = new List<StoryFragment>();

            if (timeToVote)
            {
                secondsToVote = 0;
                fragmentsThisRound.AddRange(room.FrameFragments);
            }

            return new ServerUpdate { WritersPresent = room.Writers, TimeToVote = timeToVote, SecondsToVote = secondsToVote };
        }
    }
}