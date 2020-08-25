using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Storyteller.Hubs;
using Storyteller.Models;

namespace Storyteller.Service
{
    public static class ApplicationService
    {
        private static IHubContext<StoryHub> _context;

        public static Random AppRng = new System.Random();

        public static void Initialize(IHubContext<StoryHub> context)
        {
            _context = context;
            Rooms = new List<Room>();
            Writers = new List<Writer>();
        }

        public static string SelectWinner(Dictionary<string, int> pointTotals)
        {
            var winners = pointTotals.OrderByDescending(w => w.Value);

            if (winners.Any())
            {
                var fragmentsWithTopScore = winners.Where(w => w.Value == winners.First().Value);

                var count = fragmentsWithTopScore.Count();
                var index = AppRng.Next(0, count);
                var winner = fragmentsWithTopScore.ToArray()[index];
                return winner.Key;
            }
            else
            {
                return Guid.Empty.ToString();
            }
        }

        public static void TallyVotes(Room room)
        {
            
            room.NextAction = ActionType.Vote;
            room.NextActionTime = DateTime.Now.AddMinutes(1);

            var totals = RoomService.VotesToTotals(room.FragmentVotes);
            var winner = SelectWinner(totals);

            room.Story.StoryFragments.Add(room.FrameFragments.Where(f => f.Identifier == winner).Single());

            room.FrameFragments.Clear();
            room.FragmentVotes.Clear();
            _context.Clients.Group("room-" + room.Code).SendAsync("startWriting", room);
            return;
        }

        public static void Vote(Room room)
        {

            var fragmentCount = room.FrameFragments.Count;

            if (fragmentCount >= 2)
            {
                room.NextActionTime = DateTime.Now.AddSeconds(10 * room.FrameFragments.Count);
                room.NextAction = ActionType.TallyVotes;
                _context.Clients.Group("room-" + room.Code).SendAsync("startVoting", room);
                return;
            }
            else if (fragmentCount == 1)
            {
                room.Story.StoryFragments.Add(room.FrameFragments.First());
            }

            room.NextAction = ActionType.Vote;
            room.NextActionTime = DateTime.Now.AddMinutes(1);

            room.FrameFragments.Clear();
            room.FragmentVotes.Clear();
            _context.Clients.Group("room-" + room.Code).SendAsync("startWriting", room);
            return;
        }

        public static void GameUpdate(Room room)
        {
            room.LastUpdate = DateTime.Now;
            if (!room.Started)
            {
                return;
            }

            var timeToNextAction = room.NextActionTime - DateTime.Now;

            var timeToTakeAction = room.NextActionTime <= DateTime.Now;

            var activeUsers = room.PresentWriters.Count;
            var totalFrames = room.FrameFragments.Count;
            var totalVotes = room.FragmentVotes.Count;

            if (timeToTakeAction && room.NextAction == ActionType.Vote)
            {
                Vote(room);
                return;
            }

            if (totalFrames >= activeUsers && room.NextAction == ActionType.Vote)
            {
                Vote(room);
                return;
            }

            if (timeToTakeAction && room.NextAction == ActionType.TallyVotes)
            {
                TallyVotes(room);
                return;
            }

            if (totalVotes >= activeUsers && room.NextAction == ActionType.TallyVotes)
            {
                TallyVotes(room);
                return;
            }

        }

        public static bool IsStoryUpdated(Room room, Writer writer)
        {
            var lastStory = WriterService.GetLastFragmentId(writer);

            if (lastStory == Guid.Empty.ToString())
            {
                return true;
            }

            if (room.Story.StoryFragments.Count > 0)
            {
                if (room.Story.StoryFragments.Last().Identifier != lastStory)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public static void AddWriter(Writer writer)
        {
            Writers.Add(writer);
        }

        public static void AddRoom(Room room)
        {
            Rooms.Add(room);
        }

        public static Room FindRoom(string roomId)
        {
            var rooms = Rooms.Where(r => r.Code == roomId);

            if (rooms.Any())
            {
                return rooms.Single();
            }
            else
            {
                return null;
            }
        }

        public static Writer FindWriter(string userId)
        {
            var writers = Writers.Where(w => w.Identifier == userId);

            if (writers.Any())
            {
                return writers.Single();
            }
            else
            {
                return null;
            }
        }

        public static List<Room> Rooms;
        public static List<Writer> Writers;
    }
}
