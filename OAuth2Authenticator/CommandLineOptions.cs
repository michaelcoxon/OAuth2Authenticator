using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2Authenticator
{
    public class CommandLineOptions
    {
        [Option("authorize_uri", Required = true, HelpText = "The authentication URL to display in the browser.")]
        public string AuthorizationUrl { get; set; }

        [Option("redirect_uri", Required = true, HelpText = "The redirect URL to capture the response from.")]
        public string RedirectUrl { get; set; }

        [Option("token_uri", Required = true, HelpText = "The token URL to get the tkens from.")]
        public string TokenUrl { get; set; }

        [Option("client_id", Required = true, HelpText = "The token URL to get the tkens from.")]
        public string ClientId { get; set; }

        [Option("client_secret", Required = true, HelpText = "The token URL to get the tkens from.")]
        public string ClientSecret { get; set; }

        [Option("scope", Required = true, HelpText = "The token URL to get the tkens from.")]
        public string Scope { get; set; }
    }
}
