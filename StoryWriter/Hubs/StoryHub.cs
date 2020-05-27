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
        public void Send(string name, string message, string roomcode)
        {
            // Call the broadcastMessage method to update clients.
            //Clients.All.broadcastMessage(name, message);
            Clients.Group("room-" + roomcode).broadcastMessage(name, message);
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

            // Notify other users in the room that the user left.
            Clients.Group("room-" + room.Code).userLeft(writer);
        }

        public void JoinRoom (Guid writerId, string roomCode)
        {
            // Find the writer and the room.
            WriterService.LinkWriterToConnection(writerId, Context.ConnectionId);
            RoomService.LinkRoomToConnection(roomCode, Context.ConnectionId);

            var writer = ApplicationService.FindWriter(writerId);

            // Add the user to the room.
            this.Groups.Add(this.Context.ConnectionId, "room-" + roomCode);
            
            // Notify other users in the room that the user joined.
            Clients.Group("room-" + roomCode).userJoined(writer);
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
                RoomService.RegisterVote(room, writer, fragmentId);
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