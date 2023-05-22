using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoe.models
{
    public class Move
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("move")]
        public string move { get; set; }

        [JsonProperty("player_id")]
        public int player_id { get; set; }

        [JsonProperty("game_id")]
        public int game_id { get; set; }
    }
}
