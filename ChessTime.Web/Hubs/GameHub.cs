using ChessTime.Web.Core.Model;
using ChessTime.Web.Hubs.Messages;
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
        private static readonly HashSet<Player> _players = new HashSet<Player>();

        public async Task MakeMove(MoveMessage message)
        {
            var player = Context.User.Identity.Name;
            var game = _games.FirstOrDefault(g => g.WhitePlayerId == player || g.BlackPlayerId == player);
            
            await Clients.OthersInGroup(game.Id).SendAsync("MoveMade", message);
        }

        public async Task StartGame(string color)
        {
            var player = Context.User.Identity.Name;
            var game = new Game(player, color);
            _games.Add(game);

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
            await Clients.All.SendAsync("GameCreated", game);
        }

        public async Task JoinGame(string gameId)
        {
            var player = Context.User.Identity.Name;
            var game = _games.FirstOrDefault(g => g.Id == gameId);

            game.Join(player);

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
            await Clients.Group(gameId).SendAsync("GameStarted", game);
        }

        public override async Task OnConnectedAsync()
        {
            var player = Context.User.Identity.Name;
            _players.Add(new Player { Name = player });

            await Clients.All.SendAsync("PlayerJoined", player);

            await base.OnConnectedAsync();
        }
    }
}
