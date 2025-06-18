using EchoB.Domain.Entities;
using EchoB.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Polly;
using System.Text.RegularExpressions;

namespace EchoB.Api.Hubs
{
    [Authorize]
    public class CallHub : Hub
    {
        public async Task StartCall(string receiverId, object offer)
        {
            await Clients.User(receiverId).SendAsync("ReceiveOffer", Context.UserIdentifier, offer);
        }
        public async Task AnswerCall(string callerId, object response)
        {
            await Clients.User(callerId).SendAsync("ReceiveAnswer", response);
        }
        public async Task SendIceCandidate(string otherUserId, object candidate)
        {
            await Clients.User(otherUserId).SendAsync("ReceiveIceCandidate", candidate);
        }
        public async Task EndCall(string otherUserId)
        {
            await Clients.User(otherUserId).SendAsync("CallEnded");
        }
        public async Task SendTranslatedText(string text)
        {
            // Send to the other participant in the call
            // Note: In a two-person call, we assume the other participant is the one not sending the text
            await Clients.OthersInGroup(Context.UserIdentifier).SendAsync("ReceiveTranslatedText", text);
        }
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            await base.OnConnectedAsync();
        }
       
    }
}
