using System.Text.Json;
using Fleck;
using chatty.core;
using chatty.exceptions.customExceptions;
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
        //limit the number of users in a room to 50
        if (StateService.Rooms.TryGetValue(dto.RoomId, out var guids) && guids.Count > 50)
        {
            throw new RoomLimitException("You can only join 50 rooms.");
        }
        
        StateService.AddToRoom(socket, dto.RoomId);
        
        socket.SendDto(new ServerAddsClientToRoom()
        {
            Messages = await messageService.GetRecentMessages(dto.RoomId),
            Room = dto.RoomId
        });
        
        StateService.BroadcastToRoom(dto.RoomId, new ServerBroadcastsMessageToRoom()
        {
            Message = $"{StateService.Connections[socket.ConnectionInfo.Id].Username} has joined the room.",
            Username = "Chatty Cathy",
            Timestamp = DateTime.Now.ToString("o"),
        });
    }
}

public class ServerAddsClientToRoom : BaseDto
{
    public IEnumerable<ChatMessage> Messages { get; set; }  = null!;
    public int Room { get; set; }
}






