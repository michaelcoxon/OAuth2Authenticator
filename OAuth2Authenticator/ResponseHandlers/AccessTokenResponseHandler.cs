using CommandLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2Authenticator.ResponseHandlers
{
    public class AccessTokenResponseHandler : IResponseHandler
    {
        public void Execute(string tokenJson)
        {
            Console.WriteLine(JsonConvert.DeserializeObject<Dictionary<string,string>>(tokenJson)["access_token"]);
        }
    }
}
