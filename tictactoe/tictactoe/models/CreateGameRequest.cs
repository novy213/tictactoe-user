using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoe.models
{
    public class CreateGameRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("invited_player")]
        public int? Invited_player { get; set; }
        [JsonProperty("is_password")]
        public int Is_password { get; set; }
        [JsonProperty("password")]
        public string? Password { get; set; }
    }
}
