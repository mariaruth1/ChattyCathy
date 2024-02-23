using System.Text.Json;
using Fleck;
using chatty.core;
using chatty.Infrastructure;
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
        var insertedMessage = new ChatMessage();
        try
        {
            insertedMessage = await messageService.InsertMessage(dto.Message, dto.RoomId,
                StateService.Connections[socket.ConnectionInfo.Id].Username, date);
        }
        catch (Exception)
        {
            throw new Exception("Failed to insert message.");
        }

        var message = new ServerBroadcastsMessageToRoom();
        if (insertedMessage != null)
        {
            message = new ServerBroadcastsMessageToRoom()
            {
                Message = insertedMessage.Content,
                Username = insertedMessage.Nickname,
                Timestamp = insertedMessage.Timestamp,
                //RoomId = dto.RoomId
            };
        }
        
        StateService.BroadcastToRoom(dto.RoomId, message);
    }
}

public class ServerBroadcastsMessageToRoom : BaseDto
{
    public string Message { get; set; }
    public string Username { get; set; }
    public string Timestamp { get; set; }
    
}












