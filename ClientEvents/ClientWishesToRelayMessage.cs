using Fleck;
using helloworld.ServerEvents;
using lib;

namespace helloworld.ClientEvents;

public class ClientWishesToRelayMessageDto : BaseDto
{
    public string Message { get; set; } = null!;
}

public class ClientWishesToRelayMessage : BaseEventHandler<ClientWishesToRelayMessageDto>
{
    public override Task Handle(ClientWishesToRelayMessageDto dto, IWebSocketConnection socket)
    {
        socket.SendDto(new ServerEchosClient
        {
            Message = dto.Message
        });
        return Task.CompletedTask;
    }
}