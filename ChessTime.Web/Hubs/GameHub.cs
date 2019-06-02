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
        public static readonly HashSet<Game> Games = new HashSet<Game>();
        public static readonly HashSet<Player> Players = new HashSet<Player>();

        public async Task MakeMove(MoveMessage message)
        {
            var player = Context.User.Identity.Name;
            var game = Games.FirstOrDefault(g => g.WhitePlayerId == player || g.BlackPlayerId == player);
            
            await Clients.OthersInGroup(game.Id).SendAsync("MoveMade", message);
        }

        public async Task StartGame(string color)
        {
            var player = Context.User.Identity.Name;
            await RemoveExistingGames(player);

            var game = new Game(player, color);
            Games.Add(game);

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
            await Clients.All.SendAsync("GameCreated", game);
        }

        public async Task JoinGame(string gameId)
        {
            var player = Context.User.Identity.Name;
            await RemoveExistingGames(player);

            var game = Games.FirstOrDefault(g => g.Id == gameId);
            game.Join(player);

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
            await Clients.Group(gameId).SendAsync("GameStarted", game);
        }

        public override async Task OnConnectedAsync()
        {
            var player = Context.User.Identity.Name;
            Players.Add(new Player { Name = player });

            await Clients.All.SendAsync("PlayerJoined", player);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var player = Context.User.Identity.Name;
            Players.RemoveWhere(p => p.Name == player);
            await RemoveExistingGames(player);

            await Clients.All.SendAsync("PlayerDisconnected", player);

            await base.OnDisconnectedAsync(exception);
        }

        private async Task RemoveExistingGames(string player)
        {
            foreach (var game in Games.Where(g => g.WhitePlayerId == player || g.BlackPlayerId == player))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, game.Id);
                await Clients.All.SendAsync("GameOver", player == game.WhitePlayerId ?
                        game.BlackWins() : game.WhiteWins());
            }

            Games.RemoveWhere(g => g.WhitePlayerId == player || g.BlackPlayerId == player);
        }
    }
}
