using System.Text.Json;
using Fleck;
using helloworld.core;
using helloworld.Services;
using lib;

namespace helloworld.ClientEvents;

public class ClientWantsToBroadcastToRoomDto : BaseDto
{
    public string Message { get; set; } = null!;
    public int RoomId { get; set; }
}

public class ClientWantsToBroadcastToRoom(MessageService messageService) : BaseEventHandler<ClientWantsToBroadcastToRoomDto>
{
    public override async Task Handle(ClientWantsToBroadcastToRoomDto dto, IWebSocketConnection socket)
    {
        var success = await messageService.InsertMessage(dto.Message, dto.RoomId,
            StateService.Connections[socket.ConnectionInfo.Id].Username) == 1;
        if(!success)
        {
            throw new Exception("Failed to insert message.");
            return;
        }

        var message = new ServerBroadcastsMessageWithUsername()
        {
            Message = dto.Message,
            Username = StateService.Connections[socket.ConnectionInfo.Id].Username
        };
        
        StateService.BroadcastToRoom(dto.RoomId, JsonSerializer.Serialize(message));
    }
}

public class ServerBroadcastsMessageWithUsername : BaseDto
{
    public string Message { get; set; }
    public string Username { get; set; }
}












