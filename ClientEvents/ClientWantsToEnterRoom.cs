using System.Text.Json;
using Fleck;
using helloworld.core;
using helloworld.Infrastructure;
using helloworld.Services;
using lib;

namespace helloworld.ClientEvents;

public class ClientWantsToEnterRoomDto : BaseDto
{
    public int RoomId { get; set; }
}

public class ClientWantsToEnterRoom(MessageService messageService) : BaseEventHandler<ClientWantsToEnterRoomDto>
{
    public override async Task Handle(ClientWantsToEnterRoomDto dto, IWebSocketConnection socket)
    {
        StateService.AddToRoom(socket, dto.RoomId);
        
        await socket.Send(JsonSerializer.Serialize(new ServerAddsClientToRoomDto()
        {
            Messages = await messageService.GetRecentMessages(dto.RoomId)   
        }));
    }
}

public class ServerAddsClientToRoomDto : BaseDto
{
    public IEnumerable<Message> Messages { get; set; } = new List<Message>();
}






