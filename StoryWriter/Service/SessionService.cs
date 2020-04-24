using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StoryWriter.Models;

namespace StoryWriter.Service
{
    public class SessionService
    {
        public static Writer GetWriter (object userId)
        {
            if (userId == null)
                return null;

            Guid id;

            try
            {
                id = (Guid)userId;
            }
            catch (Exception)
            {
                return null;
            }

            return ApplicationService.FindWriter(id);
        }

        public static Room GetRoom (object roomCode)
        {
            if (roomCode == null)
                return null;

            string code;

            try
            {
                code = (string)roomCode;
            }
            catch (Exception)
            {
                return null;
            }

            return ApplicationService.FindRoom(code);
        }

        public static void AddMessage (HttpSessionStateBase session, string message)
        {
            session[SessionVariables.Messages] = message;
        }
    }
}