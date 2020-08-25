using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Storyteller.Models;

namespace Storyteller.Service
{
    public static class WriterService
    {
        public static Dictionary<string, string> LastFragments = new Dictionary<string, string>();

        public static Dictionary<string, string> WriterConnections = new Dictionary<string, string>();

        public static void LinkWriterToConnection(string writerId, string connectionId)
        {
            if (WriterConnections.ContainsKey(writerId))
            {
                WriterConnections[writerId] = connectionId;
            }
            else
            {
                WriterConnections.Add(writerId, connectionId);
            }
        }

        public static Writer GetWriterFromConnection(string connectionId)
        {
            var writer = WriterConnections.Where(w => w.Value == connectionId);

            if (!writer.Any())
            {
                return null;
            }

            var writerId = writer.Single().Key;
            return ApplicationService.FindWriter(writerId);
        }

        public static Writer Create(string username)
        {
            return new Writer { Identifier = Guid.NewGuid().ToString(), Name = username, Score = 0 };
        }

        public static string GetLastFragmentId(Writer writer)
        {
            if (LastFragments.ContainsKey(writer.Identifier))
            {
                return LastFragments[writer.Identifier];
            }
            else
            {
                return Guid.Empty.ToString();
            }
        }

        public static void WriterUpdate(Writer writer, Story story)
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
