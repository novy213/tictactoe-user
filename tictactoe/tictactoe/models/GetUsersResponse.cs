using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoe.models
{
    public class GetUsersResponse : APIResponse
    {
        [JsonProperty("users")]
        public List<User> Users { get; set; }
    }
}
