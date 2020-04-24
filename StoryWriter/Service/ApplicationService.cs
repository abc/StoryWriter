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