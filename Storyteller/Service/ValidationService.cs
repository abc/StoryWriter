using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Storyteller.Models;

namespace Storyteller.Service
{
    public static class ValidationService
    {
        public static ValidationResponse ValidateRoomName(string roomName)
        {
            return new ValidationResponse { Validated = true };
        }

        public static ValidationResponse ValidateWriterName(string writerName)
        {
            return new ValidationResponse { Validated = true };
        }
    }
}
