using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessTime.Web.Core.Model
{
    public class Game
    {
        public string Id { get; private set; }
        public string WhitePlayerId { get; private set; }
        public string BlackPlayerId { get; private set; }
        public bool IsActive { get; private set; }
        public string Status { get; private set; }
        public DateTime StartDate { get; private set; }

        public Game(string creatorId, string color)
        {
            Id = Guid.NewGuid().ToString();
            Status = "Created";

            if (color == "white")
            {
                WhitePlayerId = creatorId;
            }

            if (color == "black")
            {
                BlackPlayerId = creatorId;
            }
        }

        public void Join(string playerId)
        {
            BlackPlayerId = playerId;
            Status = "Started";
            StartDate = DateTime.Now;
        }

        public Game BlackWins()
        {
            Status = "BlackWins";
            return this;
        }

        public Game WhiteWins()
        {
            Status = "WhiteWins";
            return this;
        }
    }
}
