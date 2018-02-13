using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDBResourceTokenBroker.Shared.Models
{
    public class GameResult : TypedDocument<GameResult>
    {

        [JsonProperty("WinnerName")]
        public string WinnerName { get; set; }

        [JsonProperty("WinnerScore")]
        public string WinnerScore { get; set; }

    }
}
