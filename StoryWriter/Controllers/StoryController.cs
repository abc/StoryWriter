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
    public class StoryController : Controller
    {
        // 
        public ActionResult Index()
        {
            // Check to see if this user is already logged in!

            var writer = SessionService.GetWriter(Session[SessionVariables.UserId]);

            if (writer == null)
            {
                return View();
            }

            var room = SessionService.GetRoom(Session[SessionVariables.RoomCode]);
            
            if (room == null)
            {
                return RedirectToAction("Start");
            }

            return RedirectToAction("Room", new { Id = room.Code });
        }

        public ActionResult ForgetMe()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

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

            return PartialView(room.Story);
        }

        public string Update(string Id, Guid SenderId, string Fragment, string FragmentId)
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

            ApplicationService.GameUpdate(room);

            if (!string.IsNullOrWhiteSpace(Fragment))
            {
                RoomService.RegisterFragment(room, writer, Fragment);
            }

            if (!string.IsNullOrWhiteSpace(FragmentId))
            {
                RoomService.RegisterVote(room, writer, FragmentId);
            }

            var serverUpdate = RoomService.GetUpdate(room, writer);

            // Update the writer service to track this most recent update.
            WriterService.WriterUpdate(writer, room.Story);

            Response.ContentType = "application/json";
            return JsonConvert.SerializeObject(serverUpdate);
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

            room.PresentWriters.RemoveAll(w => w.Identifier == writer.Identifier);
            room.AbsentWriters.Add(writer);

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

            if (!room.PresentWriters.Exists(w => w.Identifier == writer.Identifier))
            {
                room.PresentWriters.Add(writer);
            }

            if (room.AbsentWriters.Exists(w => w.Identifier == writer.Identifier))
            {
                room.AbsentWriters.RemoveAll(w => w.Identifier == writer.Identifier);
            }

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
            room.PresentWriters.Add(writer);

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

            return View(room);
        }
    }
}