using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StoryWriter.Models;

namespace StoryWriter.Service
{
    public static class ApplicationService
    {
        public static void Initialize()
        {
            Rooms = new List<Room>();
            Writers = new List<Writer>();
        }

        public static void GameUpdate(Room room)
        {
            // Only update every ten seconds.
            if (!(room.LastUpdate.AddSeconds(10) <= DateTime.Now))
            {
                return;
            }

            room.LastUpdate = DateTime.Now;

            var timeToNextAction = room.NextActionTime - DateTime.Now;
            var seconds = timeToNextAction.Seconds;

            var timeToTakeAction = room.NextActionTime <= DateTime.Now;

            if (timeToTakeAction && room.NextAction == ActionType.Vote)
            {
                room.NextActionTime = DateTime.Now.AddSeconds(10 * room.FrameFragments.Count);
                room.NextAction = ActionType.TallyVotes;
                return;
            }

            if (timeToTakeAction && room.NextAction == ActionType.TallyVotes)
            {
                room.NextAction = ActionType.Vote;
                room.NextActionTime = DateTime.Now.AddMinutes(1);

                var totals = RoomService.VotesToTotals(room.FragmentVotes);
                var winner = totals.OrderByDescending(w => w.Value).First();

                room.Story.StoryFragments.Add(room.FrameFragments.Where(f => f.Identifier == winner.Key).Single());
                room.FrameFragments.Clear();
                room.FragmentVotes.Clear();
                return;
            }
            
        }

        public static bool IsStoryUpdated (Room room, Writer writer)
        {
            var lastStory = WriterService.GetLastFragmentId(writer);

            if (lastStory == Guid.Empty)
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

        public static Writer FindWriter(Guid userId)
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