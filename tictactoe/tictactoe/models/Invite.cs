using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoe.models
{
    public class Invite
    {
        [JsonProperty("game_id")]
        public int game_id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("user_name")]
        public string user_name { get; set; }

        [JsonProperty("user_last_name")]
        public string user_last_name { get; set; }
    }
}
