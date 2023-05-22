using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoe.models
{
    public class GetGameInfoResponse : APIResponse
    {
        [JsonProperty("game")]
        public Game Game { get; set; }
    }
}
