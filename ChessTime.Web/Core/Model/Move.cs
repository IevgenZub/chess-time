using System;

namespace ChessTime.Web.Core.Model
{
    public class Move
    {
        public string Id { get; set; }
        public string GameId { get; set; }
        public string PlayerId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Color { get; set; }
        public string Piece { get; set; }
        public DateTime CreateDate { get; set; }
    }
}