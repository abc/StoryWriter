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
    }
}