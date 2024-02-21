using System.Text.Json;
using Fleck;
using chatty.core;
using chatty.Services;
using lib;

namespace chatty.ClientEvents;

public class ClientWantsToBroadcastToRoomDto : BaseDto
{
    public string Message { get; set; } = null!;
    public int RoomId { get; set; }
}

public class ClientWantsToBroadcastToRoom(MessageService messageService) : BaseEventHandler<ClientWantsToBroadcastToRoomDto>
{
    public override async Task Handle(ClientWantsToBroadcastToRoomDto dto, IWebSocketConnection socket)
    {
        var date = DateTime.Now;
        var success = await messageService.InsertMessage(dto.Message, dto.RoomId,
            StateService.Connections[socket.ConnectionInfo.Id].Username, date) == 1;
        if(!success)
        {
            throw new Exception("Failed to insert message.");
            return;
        }

        var message = new ServerBroadcastsMessageToRoom()
        {
            Message = dto.Message,
            Username = StateService.Connections[socket.ConnectionInfo.Id].Username,
            Timestamp = date.ToShortTimeString(),
            RoomId = dto.RoomId
        };
        
        StateService.BroadcastToRoom(dto.RoomId, message);
    }
}

public class ServerBroadcastsMessageToRoom : BaseDto
{
    public string Message { get; set; }
    public string Username { get; set; }
    public string Timestamp { get; set; }
    public int RoomId { get; set; }
}












