using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessTime.Web.Core.Model
{
    public class Player
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Rating { get; set; }
    }
}
