using ChessTime.Web.Core.Model;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessTime.Web.Hubs
{
    public class GameHub : Hub
    {
        private static readonly HashSet<Game> _games = new HashSet<Game>();

        public async Task SendMove(MoveMessage message)
        {
            var player = Context.User.Identity.Name;
            var game = _games.FirstOrDefault(g => g.WhitePlayerId == player || g.BlackPlayerId == player);
            
            await Clients.OthersInGroup(game.Id).SendAsync("ReceiveMove", message);
        }

        public override async Task OnConnectedAsync()
        {
            var player = Context.User.Identity.Name;
            var game = _games.FirstOrDefault(g => g.Status == "Created");
            
            if (game == null)
            {
                game = new Game(player);
                _games.Add(game);
                await Clients.Caller.SendAsync("StartGame", "white");
            }
            else
            {
                if (game.WhitePlayerId != player)
                {
                    game.Join(player);
                    await Clients.Caller.SendAsync("StartGame", "black");
                }
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
            await base.OnConnectedAsync();
        }
    }
}
