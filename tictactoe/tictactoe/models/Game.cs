using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoe.models
{
    public class Game
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("host_id")]
        public int Host_id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("enemy_id")]
        public int? Enemy_id { get; set; }
        [JsonProperty("invited_player")]
        public int? Invited_player { get; set; }
        [JsonProperty("is_password")]
        public bool Is_password { get; set; }
        [JsonProperty("password")]
        public string? Password { get; set; }
        [JsonProperty("user_name")]
        public string User_name { get; set; }
        [JsonProperty("user_last_name")]
        public string User_last_name { get; set; }
    }
}
