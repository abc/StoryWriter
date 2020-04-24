using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StoryWriter.Models;

namespace StoryWriter.Service
{
    public static class WriterService
    {
        public static Writer Create(string username)
        {
            return new Writer { Identifier = Guid.NewGuid(), Name = username, Score = 0 };
        }
    }
}