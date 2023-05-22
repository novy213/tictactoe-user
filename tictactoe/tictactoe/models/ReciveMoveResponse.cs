using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoe.models
{
    public class ReciveMoveResponse : APIResponse
    {
        [JsonProperty("moves")]
        public List<Move> Moves { get; set; }
    }
}
