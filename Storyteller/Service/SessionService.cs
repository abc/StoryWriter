using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Storyteller.Models;

namespace Storyteller.Service
{
    public class SessionService
    {
        public static Writer GetWriter(object userId)
        {
            if (userId == null)
                return null;

            if (!(userId is string))
            {
                return null;
            }

            string id;

            try
            {
                id = (string)userId;
            }
            catch (Exception ex)
            {
                return null;
            }

            return ApplicationService.FindWriter(id);
        }

        public static Room GetRoom(object roomCode)
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

        public static void AddMessage(ISession session, string message)
        {
            session.SetString(SessionVariables.Messages, message);
        }
    }
}
