using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoe.models
{
    public class GetGamesResponse : APIResponse
    {
        [JsonProperty("games")]
        public List<Game> Games;
    }
}
