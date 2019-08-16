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
        [Option("authorize_uri", Required = true, HelpText = "The authorization URL to display in the browser.")]
        public string AuthorizationUrl { get; set; }

        [Option("redirect_uri", Required = true, HelpText = "The redirect URL to capture the response from.")]
        public string RedirectUrl { get; set; }

        [Option("token_uri", Required = true, HelpText = "The token URL to get the tokens from.")]
        public string TokenUrl { get; set; }

        [Option("client_id", Required = true, HelpText = "The client identifier.")]
        public string ClientId { get; set; }

        [Option("client_secret", Required = true, HelpText = "The client secret.")]
        public string ClientSecret { get; set; }

        [Option("scope", Required = true, HelpText = "The scope of the access request.")]
        public string Scope { get; set; }

        [Option("response_handler", Required = false, HelpText = "Defines what to do with the result.")]
        public ResponseHandlerEnum ResponseHandler { get; set; } = ResponseHandlerEnum.StdOut;
    }

    public enum ResponseHandlerEnum
    {
        StdOut,
    }
}
