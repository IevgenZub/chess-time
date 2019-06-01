using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessTime.Web.Core.Model;
using ChessTime.Web.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace ChessTime.Web.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public IEnumerable<Player> Players { get; set; }
        public IEnumerable<Game> Games { get; set; }

        public void OnGet()
        {
            Players = GameHub.Players;
            Games = GameHub.Games;
        }
    }
}
