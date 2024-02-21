using System.Text.Json;
using Fleck;
using chatty.core;
using chatty.Infrastructure;
using chatty.Services;
using lib;

namespace chatty.ClientEvents;

public class ClientWantsToEnterRoomDto : BaseDto
{
    public int RoomId { get; set; }
}

public class ClientWantsToEnterRoom(MessageService messageService) : BaseEventHandler<ClientWantsToEnterRoomDto>
{
    public override async Task Handle(ClientWantsToEnterRoomDto dto, IWebSocketConnection socket)
    {
        StateService.AddToRoom(socket, dto.RoomId);
        
        await socket.Send(JsonSerializer.Serialize(new ServerAddsClientToRoom()
        {
            Messages = await messageService.GetRecentMessages(dto.RoomId),
        }));
    }
}

public class ServerAddsClientToRoom : BaseDto
{
    public List<ChatMessage> Messages { get; set; } = new();
    //public IEnumerable<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}






