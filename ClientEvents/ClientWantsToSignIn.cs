using Fleck;
using chatty.Services;
using lib;

namespace chatty.ClientEvents;

public class ClientWantsToSignInDto : BaseDto
{
    public string Username { get; set; } = "";
}

public class ClientWantsToSignIn : BaseEventHandler<ClientWantsToSignInDto>
{
    public override Task Handle(ClientWantsToSignInDto dto, IWebSocketConnection socket)
    {
        StateService.Connections[socket.ConnectionInfo.Id].Username = dto.Username;
        socket.SendDto(new ServerConfirmsSignIn
        {
            Username = dto.Username
        });
        return Task.CompletedTask;
    }
}

public class ServerConfirmsSignIn : BaseDto
{
    public string Username { get; set; } = "";
}