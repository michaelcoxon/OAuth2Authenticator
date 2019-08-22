using CommandLine;
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
        private readonly EnvironmentVariableCommandLineOptions _options;

        public class EnvironmentVariableCommandLineOptions : CommandLineOptions
        {
            [Option("prefix", Required = false, HelpText = "Prefix for the environment variables")]
            public string Prefix { get; set; } = "OAuth2.";
        }

        public string Prefix
        {
            get => this._options.Prefix;
            set => this._options.Prefix = value;
        }

        public EnvironmentVariableResponseHandler(EnvironmentVariableCommandLineOptions options)
        {
            this._options = options;
        }

        public void Execute(string tokenJson)
        {
            var tokens = JsonConvert.DeserializeObject<Dictionary<string, string>>(tokenJson);

            foreach (var kvp in tokens)
            {
                Environment.SetEnvironmentVariable($"{this.Prefix}{kvp.Key}", kvp.Value);
            }
        }
    }
}
