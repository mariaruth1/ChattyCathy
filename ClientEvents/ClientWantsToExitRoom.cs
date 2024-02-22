using chatty.Infrastructure;
using chatty.Services;
using Fleck;
using lib;

namespace chatty.ClientEvents;

public class ClientWantsToExitRoomDto: BaseDto
{
    public int Room { get; set; }
}

public class ClientWantsToExitRoom: BaseEventHandler<ClientWantsToExitRoomDto>
{
    public override Task Handle(ClientWantsToExitRoomDto dto, IWebSocketConnection socket)
    {
        StateService.RemoveFromRoom(socket, dto.Room);
        socket.SendDto(new ServerConfirmsExitRoom
        {
            Message = "You have left the room."
        });
        StateService.BroadcastToRoom(dto.Room, new ServerBroadcastsMessageToRoom()
            {
                Message = $"{StateService.Connections[socket.ConnectionInfo.Id].Username} has left the room.",
                Username = "Chatty Cathy",
                Timestamp = DateTime.Now.ToString("o"),
            });
        return Task.CompletedTask;
    }
}

public class ServerConfirmsExitRoom: BaseDto
{
    public string Message { get; set; } = null!;
}