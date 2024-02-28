using System.Data;
using System.Text.Json;
using chatty.exceptions.customExceptions;
using chatty.exceptions.events;
using Fleck;
using chatty.Services;
using lib;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace chatty.ClientEvents;

public class ClientWantsToSignInDto : BaseDto
{
    public string Username { get; set; } = "";
}

public class ClientWantsToSignIn : BaseEventHandler<ClientWantsToSignInDto>
{
    public override Task Handle(ClientWantsToSignInDto dto, IWebSocketConnection socket)
    {
        if(dto.Username.Length <3)
        {
            throw new InvalidUsernameException("Username is invalid.");
        }
        
        if (StateService.Connections.Values.Any(x => x.Username == dto.Username))
        {
            throw new DuplicateNameException("Username is taken.");
        }
        
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