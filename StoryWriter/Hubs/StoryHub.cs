using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using StoryWriter.Service;

namespace StoryWriter.Hubs
{
    public class StoryHub : Hub
    {
        public void SendUpdate (Models.Room room)
        {
            Clients.Group("room-" + room.Code).update(room);
        }

        public void Send(string name, string message, string roomcode)
        {
            // Call the broadcastMessage method to update clients.
            //Clients.All.broadcastMessage(name, message);
            Clients.Group("room-" + roomcode).broadcastMessage(name, message);
        }

        public void StartGame()
        {
            var writer = WriterService.GetWriterFromConnection(Context.ConnectionId);
            var room = RoomService.GetRoomFromConnection(Context.ConnectionId);

            if ((writer == null) || (room == null))
            {
                throw new InvalidOperationException();
            }

            if (room.Owner.Identifier != writer.Identifier)
            {
                throw new InvalidOperationException();
            }

            if (room.Started)
            {
                throw new InvalidOperationException();
            }

            room.Start();
            Clients.Group("room-" + room.Code).gameStarted(room);
        }

        public override Task OnDisconnected(bool stopCalled)
        {

            // Find the writer and the room.
            var writer = WriterService.GetWriterFromConnection(Context.ConnectionId);
            var room = RoomService.GetRoomFromConnection(Context.ConnectionId);

            if ((writer != null) || (room != null))
            {
                // Remove the user from the room.
                this.Groups.Remove(Context.ConnectionId, "room-" + room.Code);

                room.PresentWriters.RemoveAll(w => w.Identifier == writer.Identifier);
                room.AbsentWriters.Add(writer);

                // Notify other users in the room that the user left.
                Clients.Group("room-" + room.Code).userLeft(room);
            }

            return base.OnDisconnected(stopCalled);
        }

        public void LeaveRoom ()
        {
            // Find the writer and the room.
            var writer = WriterService.GetWriterFromConnection(Context.ConnectionId);
            var room = RoomService.GetRoomFromConnection(Context.ConnectionId);

            if ((writer == null) || (room == null))
            {
                throw new InvalidOperationException();
            }

            // Remove the user from the room.
            this.Groups.Remove(Context.ConnectionId, "room-" + room.Code);

            room.PresentWriters.RemoveAll(w => w.Identifier == writer.Identifier);
            room.AbsentWriters.Add(writer);

            // Notify other users in the room that the user left.
            Clients.Group("room-" + room.Code).userLeft(room);
        }

        public void JoinRoom (Guid writerId, string roomCode)
        {
            // Find the writer and the room.
            WriterService.LinkWriterToConnection(writerId, Context.ConnectionId);
            RoomService.LinkRoomToConnection(roomCode, Context.ConnectionId);

            var writer = ApplicationService.FindWriter(writerId);
            var room = ApplicationService.FindRoom(roomCode);

            // Add the user to the room.
            this.Groups.Add(this.Context.ConnectionId, "room-" + roomCode);

            room.AbsentWriters.RemoveAll(w => w.Identifier == writer.Identifier);
            room.PresentWriters.Add(writer);

            // Notify other users in the room that the user joined.
            Clients.Group("room-" + roomCode).userJoined(room);
            Clients.Client(Context.ConnectionId).welcome(room);
        }

        public void CastVote(string fragmentId)
        {
            var writer = WriterService.GetWriterFromConnection(Context.ConnectionId);
            var room = RoomService.GetRoomFromConnection(Context.ConnectionId);

            if ((writer == null) || (room == null))
            {
                throw new InvalidOperationException();
            }

            if (!string.IsNullOrWhiteSpace(fragmentId))
            {
                var fragment = room.FrameFragments.Where(f => f.Identifier.ToString() == fragmentId).Single();
                if (fragment.Author.Identifier != writer.Identifier)
                {
                    // Refuse to register votes for fragments the user wrote themselves.
                    RoomService.RegisterVote(room, writer, fragmentId);
                }
            }

            // Notify the other users of the cast vote.
            Clients.Group("room-" + room.Code).voteCast(writer.Identifier, fragmentId);
        }

        public void SubmitFragment(string fragment)
        {
            var writer = WriterService.GetWriterFromConnection(Context.ConnectionId);
            var room = RoomService.GetRoomFromConnection(Context.ConnectionId);

            if ((writer == null) || (room == null))
            {
                throw new InvalidOperationException();
            }

            if (!string.IsNullOrWhiteSpace(fragment))
            {
                RoomService.RegisterFragment(room, writer, fragment);
            }

            // Notify the other users of the new fragment.
            Clients.Group("room-" + room.Code).newFragment(fragment);
        }
    }
}