using CommandLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2Authenticator.ResponseHandlers
{
    public class StdOutResponseHandler : IResponseHandler
    {
        private readonly StdOutCommandLineOptions _options;

        public class StdOutCommandLineOptions : CommandLineOptions
        {
            [Option("pretty", Required = false, HelpText = "Dump the JSON indented")]
            public bool PrettyPrint { get; set; } = false;
        }

        public StdOutResponseHandler(StdOutCommandLineOptions commandLineOptions)
        {
            this._options = commandLineOptions;
        }

        public void Execute(string tokenJson)
        {
            if (this._options.PrettyPrint)
            {
                tokenJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(tokenJson), Formatting.Indented);
            }

            Console.WriteLine(tokenJson);
        }
    }
}
