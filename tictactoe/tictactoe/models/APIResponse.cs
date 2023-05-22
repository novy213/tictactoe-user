using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoe.models
{
    public class APIResponse
    {
        [JsonProperty("error")]
        public bool Error { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
