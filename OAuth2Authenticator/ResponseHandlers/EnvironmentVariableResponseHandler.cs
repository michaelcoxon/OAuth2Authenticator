using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2Authenticator.ResponseHandlers
{
    public class EnvironmentVariableResponseHandler : IResponseHandler
    {
        private readonly string _prefix = "OAuth2.";
        public void Execute(string tokenJson)
        {
            var tokens = JsonConvert.DeserializeObject<Dictionary<string, string>>(tokenJson);

            foreach (var kvp in tokens)
            {
                Environment.SetEnvironmentVariable($"{this._prefix}{kvp.Key}", kvp.Value);
            }
        }
    }
}
