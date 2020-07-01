using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StoryWriter.Models;
using StoryWriter.Service;
using Newtonsoft.Json;

namespace StoryWriter.Controllers
{
    /// <summary>
    /// The main controller for the project, the story controller handles a lot of the application logic.
    /// </summary>
    public class StoryController : Controller
    {
        /// <summary>
        /// The entry-point for the application. If the user is logged in, this will
        /// forward them to their active room or the "Join/Host Room" page.
        /// Otherwise, it asks them to sign in.
        /// </summary>
        /// <returns>
        /// The Index view, which is a login page, or a redirect to either the "Join/Host Room"
        /// or their current room if they're in a room.
        /// </returns>
        public ActionResult Index()
        {
            // Check to see if this user is already logged in!
            var writer = SessionService.GetWriter(Session[SessionVariables.UserId]);

            if (writer == null)
            {
                // Get the user to login
                return View();
            }

            // Does the user already have a room?
            var room = SessionService.GetRoom(Session[SessionVariables.RoomCode]);
            
            if (room == null)
            {
                // If not, let them join or create a room.
                return RedirectToAction("Start");
            }

            // If they're in a room, redirect them to it.
            return RedirectToAction("Room", new { Id = room.Code });
        }

        /// <summary>
        /// Clear the user's session, allowing them to start fresh.
        /// </summary>
        /// <returns></returns>
        public ActionResult ForgetMe()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Get the current story for a given room and render it into a partial view.
        /// </summary>
        /// <param name="Id">Room code to get the story for.</param>
        /// <returns>
        /// A partial view displaying the room's story.
        /// </returns>
        public ActionResult StoryText (string Id)
        {
            // ID = Room code.
            var roomCode = Id.ToUpperInvariant();

            // Try to find the room.
            var room = ApplicationService.FindRoom(roomCode);

            if (room == null)
            {
                SessionService.AddMessage(Session, "A room with that code could not be found, please check the code and try again.");
                return null;
            }

            // Return a partial view containing the room's story so far.
            return PartialView(room.Story);
        }

        public ActionResult LeaveRoom(string Id)
        {
            // ID = Room code.
            var roomCode = Id.ToUpperInvariant();

            var writer = SessionService.GetWriter(Session[SessionVariables.UserId]);

            if (writer == null)
            {
                throw new InvalidOperationException("Unable to join a room, you are unidentified.");
            }

            // Try to find the room.
            var room = ApplicationService.FindRoom(roomCode);

            if (room == null)
            {
                SessionService.AddMessage(Session, "A room with that code could not be found, please check the code and try again.");
                return null;
            }

            Session[SessionVariables.RoomCode] = null;

            // room.PresentWriters.RemoveAll(w => w.Identifier == writer.Identifier);
            // room.AbsentWriters.Add(writer);

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index (string Id)
        {
            // ID = User name.
            var userName = Id;

            if (!ValidationService.ValidateWriterName(userName).Validated)
            {
                SessionService.AddMessage(Session, "Your name was invalid.");
                return View();
            }

            var writer = WriterService.Create(userName);
            ApplicationService.AddWriter(writer);
            Session[SessionVariables.UserId] = writer.Identifier;

            return RedirectToAction("Start");
        }

        public ActionResult Start()
        {
            // Get writer who just put in their name.
            var writer = SessionService.GetWriter(Session[SessionVariables.UserId]);

            if (writer == null)
            {
                throw new InvalidOperationException("Unable to continue, you are unidentified -- make sure cookies are enabled!");
            }

            return View(writer);
        }

        public ActionResult Join()
        {
            // Get writer who just put in their name.
            var writer = SessionService.GetWriter(Session[SessionVariables.UserId]);

            if (writer == null)
            {
                throw new InvalidOperationException("Unable to continue, you are unidentified -- make sure cookies are enabled!");
            }

            return View(writer);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Join (string Id)
        {
            // ID = Room code.
            var roomCode = Id.ToUpperInvariant();

            var writer = SessionService.GetWriter(Session[SessionVariables.UserId]);

            if (writer == null)
            {
                throw new InvalidOperationException("Unable to join a room, you are unidentified.");
            }

            // Try to find the room.
            var room = ApplicationService.FindRoom(roomCode);

            if (room == null)
            {
                SessionService.AddMessage(Session, "A room with that code could not be found, please check the code and try again.");
                return View(writer);
            }

            if (writer.Color != null)
            {
                if (room.ColorsInUse.Any(c => c.Name == writer.Color.Name))
                {
                    writer.Color = ColorService.RandomColor(room.ColorsInUse);
                }
            }
            else
            {
                writer.Color = ColorService.RandomColor(room.ColorsInUse);
            }

            room.ColorsInUse.Add(writer.Color);

            /*
            if (!room.PresentWriters.Exists(w => w.Identifier == writer.Identifier))
            {
                room.PresentWriters.Add(writer);
            }

            if (room.AbsentWriters.Exists(w => w.Identifier == writer.Identifier))
            {
                room.AbsentWriters.RemoveAll(w => w.Identifier == writer.Identifier);
            }
            */

            return RedirectToAction("Room", new { Id = room.Code });
        }

        public ActionResult Create()
        {
            var writer = SessionService.GetWriter(Session[SessionVariables.UserId]);

            if (writer == null)
            {
                throw new InvalidOperationException("Unable to continue, you are unidentified -- make sure cookies are enabled!");
            }

            return View(writer);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(string Id)
        {
            // ID = Room Name
            var roomName = Id;

            // Get writer who is creating the room.
            var writer = SessionService.GetWriter(Session[SessionVariables.UserId]);

            if (writer == null)
            {
                throw new InvalidOperationException("Unable to create a room, you are unidentified.");
            }

            if (!ValidationService.ValidateRoomName(roomName).Validated)
            {
                SessionService.AddMessage(Session, "Your room name was invalid.");
                return View(writer);
            }

            var room = RoomService.Create(roomName, writer);
            ApplicationService.AddRoom(room);

            // room.Writers.Add(writer);
            // room.PresentWriters.Add(writer);

            if (writer.Color != null)
            {
                if (room.ColorsInUse.Any(c => c.Name == writer.Color.Name))
                {
                    writer.Color = ColorService.RandomColor(room.ColorsInUse);
                }
            }
            else
            {
                writer.Color = ColorService.RandomColor(room.ColorsInUse);
            }

            room.ColorsInUse.Add(writer.Color);

            return RedirectToAction("FirstLine", new { Id = room.Code });
        }

        public ActionResult FirstLine (string Id)
        {
            // ID = Room Code
            var roomCode = Id;

            var room = ApplicationService.FindRoom(roomCode);

            if (room == null)
            {
                SessionService.AddMessage(Session, "A room with that code could not be found, please check the code and try again.");
                return View();
            }

            return View(room);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FirstLine (string Id, string firstLine)
        {
            var roomCode = Id;

            var room = ApplicationService.FindRoom(roomCode);

            if (room == null)
            {
                SessionService.AddMessage(Session, "A room with that code could not be found, please check the code and try again.");
                return View();
            }

            room.Story.Intro = new StoryFragment { Author = room.Owner, Text = firstLine, Ending = false };

            return RedirectToAction("Room", new { Id = roomCode });
        }

        public ActionResult Room (string Id = "")
        {
            var roomCode = Id;

            var room = ApplicationService.FindRoom(roomCode);

            if (room == null)
            {
                SessionService.AddMessage(Session, "A room with that code could not be found, please check the code and try again.");
                return View();
            }

            Session[SessionVariables.RoomCode] = room.Code;

            var writer = SessionService.GetWriter(Session[SessionVariables.UserId]);

            if (writer == null)
            {
                throw new InvalidOperationException("Unable to join a room, you are unidentified.");
            }

            ViewBag.Writer = writer;

            return View(room);
        }
    }
}