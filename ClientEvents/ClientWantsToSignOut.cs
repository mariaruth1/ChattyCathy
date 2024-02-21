using chatty.Services;
using Fleck;
using lib;

namespace chatty.ClientEvents;

public class ClientWantsToSignOutDto: BaseDto
{
    
}

public class ClientWantsToSignOut: BaseEventHandler<ClientWantsToSignOutDto>
{
    public override Task Handle(ClientWantsToSignOutDto dto, IWebSocketConnection socket)
    {
        StateService.Connections[socket.ConnectionInfo.Id].Username = null;
        socket.SendDto(new ServerConfirmsSignOut
        {
            Message = "You have signed out."
        });
        return Task.CompletedTask;
    }
}

public class ServerConfirmsSignOut: BaseDto
{
    public string Message { get; set; } = null!;
}