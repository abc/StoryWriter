﻿using System;
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

            if (!string.IsNullOrWhiteSpace(Fragment))
            {
                RoomService.RegisterFragment(room, writer, Fragment);
            }

            if (!string.IsNullOrWhiteSpace(FragmentId))
            {
                RoomService.RegisterVote(room, writer, FragmentId);
            }

            var serverUpdate = RoomService.GetUpdate(room);

            Response.ContentType = "application/json";
            return JsonConvert.SerializeObject(serverUpdate);
        }

        public ActionResult LeaveRoom()
        {
            Session[SessionVariables.RoomCode] = null;
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

            if (!room.Writers.Exists(w => w.Identifier == writer.Identifier))
            {
                room.Writers.Add(writer);
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

            room.Writers.Add(writer);

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

        public ActionResult Room (string Id)
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

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddLine(string Id, string nextLine)
        {
            var roomCode = Id;

            var room = ApplicationService.FindRoom(roomCode);

            if (room == null)
            {
                SessionService.AddMessage(Session, "A room with that code could not be found, please check the code and try again.");
                return View();
            }

            var writer = SessionService.GetWriter(Session[SessionVariables.UserId]);

            if (writer == null)
            {
                throw new InvalidOperationException("Unable to create a room, you are unidentified.");
            }

            room.Story.StoryFragments.Add(new StoryFragment { Author = writer, Text = nextLine, Ending = false });

            return RedirectToAction("Room", new { Id = roomCode });
        }
    }
}