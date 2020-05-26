using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StoryWriter.Models;

namespace StoryWriter.Service
{
    public static class WriterService
    {
        public static Dictionary<Guid, Guid> LastFragments = new Dictionary<Guid, Guid>();

        public static Writer Create(string username)
        {
            return new Writer { Identifier = Guid.NewGuid(), Name = username, Score = 0 };
        }

        public static Guid GetLastFragmentId (Writer writer)
        {
            if (LastFragments.ContainsKey(writer.Identifier))
            {
                return LastFragments[writer.Identifier];
            }
            else
            {
                return Guid.Empty;
            }
        }

        public static void WriterUpdate (Writer writer, Story story)
        {
            if (story.StoryFragments.Count > 0)
            {
                var update = story.StoryFragments.Last().Identifier;

                if (LastFragments.ContainsKey(writer.Identifier))
                {
                    LastFragments[writer.Identifier] = update;
                }
                else
                {
                    LastFragments.Add(writer.Identifier, update);
                }
            }
        }
    }
}