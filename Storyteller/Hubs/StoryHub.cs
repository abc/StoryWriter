using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Storyteller.Service;
using Storyteller.Models;

namespace Storyteller.Hubs
{
    public class StoryHub : Hub
    {
        public void SendUpdate(Models.Room room)
        {
            Clients.Group("room-" + room.Code).SendAsync("update", room);
        }

        public void Send(string name, string message, string roomcode)
        {
            // Call the broadcastMessage method to update clients.
            //Clients.All.broadcastMessage(name, message);
            Clients.Group("room-" + roomcode).SendAsync("broadcastMessage", name, message);
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
            Clients.Group("room-" + room.Code).SendAsync("gameStarted", room);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // Find the writer and the room.
            var writer = WriterService.GetWriterFromConnection(Context.ConnectionId);
            var room = RoomService.GetRoomFromConnection(Context.ConnectionId);

            if ((writer != null) || (room != null))
            {
                // Remove the user from the room.
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "room" + room.Code);

                room.PresentWriters.RemoveAll(w => w.Identifier == writer.Identifier);
                room.AbsentWriters.Add(writer);

                // Notify other users in the room that the user left.
                Clients.Group("room-" + room.Code).SendAsync("userLeft", room);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public void LeaveRoom()
        {
            // Find the writer and the room.
            var writer = WriterService.GetWriterFromConnection(Context.ConnectionId);
            var room = RoomService.GetRoomFromConnection(Context.ConnectionId);

            if ((writer == null) || (room == null))
            {
                throw new InvalidOperationException();
            }

            // Remove the user from the room.
            Groups.RemoveFromGroupAsync(Context.ConnectionId, "room" + room.Code);

            room.PresentWriters.RemoveAll(w => w.Identifier == writer.Identifier);
            room.AbsentWriters.Add(writer);

            // Notify other users in the room that the user left.
            Clients.Group("room-" + room.Code).SendAsync("userLeft", room);
        }

        public void JoinRoom(string writerId, string roomCode)
        {
            // Find the writer and the room.
            WriterService.LinkWriterToConnection(writerId, Context.ConnectionId);
            RoomService.LinkRoomToConnection(roomCode, Context.ConnectionId);

            var writer = ApplicationService.FindWriter(writerId);
            var room = ApplicationService.FindRoom(roomCode);

            // Add the user to the room.
            Groups.AddToGroupAsync(Context.ConnectionId, "room-" + roomCode);

            room.AbsentWriters.RemoveAll(w => w.Identifier == writer.Identifier);
            room.PresentWriters.Add(writer);

            // Notify other users in the room that the user joined.
            Clients.Group("room-" + roomCode).SendAsync("userJoined", room);
            Clients.Client(Context.ConnectionId).SendAsync("welcome", room);
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
            Clients.Group("room-" + room.Code).SendAsync("voteCast", writer.Identifier, fragmentId);
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
            Clients.Group("room-" + room.Code).SendAsync("newFragment", fragment);
        }
    }
}
