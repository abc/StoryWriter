using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StoryWriter.Models;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace StoryWriter.Service
{
    public static class RoomService
    {
        public static Dictionary<string, string> RoomConnections = new Dictionary<string, string>();

        public static void LinkRoomToConnection(string roomCode, string connectionId)
        {
            if (RoomConnections.ContainsKey(roomCode))
            {
                RoomConnections[connectionId] = roomCode;
            }
            else
            {
                RoomConnections.Add(connectionId, roomCode);
            }
        }

        public static Room GetRoomFromConnection(string connectionId)
        {
            var room = RoomConnections.Where(w => w.Key == connectionId);

            if (!room.Any())
            {
                return null;
            }

            var roomCode = room.Single().Value;
            return ApplicationService.FindRoom(roomCode);
        }

        /// <summary>
        /// Generate a four-character code used to join a session in progress.
        /// </summary>
        /// <returns></returns>
        public static string GenerateCode()
        {
            const string pool = "abcdefghijklmnopqrstuvwxyz";
            var builder = new StringBuilder();

            for (var i = 0; i < 4; i++)
            {
                var c = pool[ApplicationService.AppRng.Next(0, pool.Length)];
                builder.Append(c);
            }

            return builder.ToString().ToUpperInvariant();
        }

        public static Room Create (string roomName, Writer owner)
        {
            var room = new Room { Code = GenerateCode(), Name = roomName, Owner = owner };
            Task taskA = new Task(() => { while (true) { ApplicationService.GameUpdate(room); Thread.Sleep(1000); } });
            taskA.Start();
            return room;
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

        public static ServerUpdate GetUpdate (Room room, Writer writer)
        {
            var secondsToAction = (room.NextActionTime - DateTime.Now).Seconds;
            var timeToAction = secondsToAction <= 0;
            var fragmentsThisRound = new List<StoryFragment>();
            fragmentsThisRound.AddRange(room.FrameFragments);
            var storyUpdated = ApplicationService.IsStoryUpdated(room, writer);

            return new ServerUpdate
            {
                PresentWriters = room.PresentWriters,
                AbsentWriters = room.AbsentWriters,
                TimeToVote = timeToAction,
                SecondsToVote = secondsToAction,
                FragmentsThisRound = room.FrameFragments,
                StoryUpdated = storyUpdated
            };
        }

        public static Dictionary<Guid, int> VotesToTotals (Dictionary<Guid, Guid> votes)
        {
            var result = new Dictionary<Guid, int>();

            foreach (var vote in votes)
            {
                if (result.ContainsKey(vote.Value))
                {
                    result[vote.Value]++;
                }
                else
                {
                    result[vote.Value] = 1;
                }
            }

            return result;
        }
    }
}