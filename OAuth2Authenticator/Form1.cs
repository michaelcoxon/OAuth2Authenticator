using CommandLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace OAuth2Authenticator
{
    public partial class Form1 : Form
    {
        private CommandLineOptions _options;

        public Form1()
        {
            Parser.Default.ParseArguments<CommandLineOptions>(Environment.GetCommandLineArgs()).WithParsed(opts => this._options = opts);

            this.InitializeComponent();

            this.webBrowser1.DocumentTitleChanged += (s, e) =>
            {
                this.Text = this.webBrowser1.DocumentTitle;
            };

            this.webBrowser1.AllowWebBrowserDrop = false;

            var runOnce = true;

            this.webBrowser1.ProgressChanged += (s, e) =>
            {
                if (runOnce && (this.webBrowser1.Url?.ToString().StartsWith(this._options.RedirectUrl, StringComparison.InvariantCultureIgnoreCase) ?? false))
                {
                    runOnce = false;
                    Task.Run(() => this.HandleResponseAsync(this.webBrowser1.Url));
                }
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.webBrowser1.Navigate(this.CreateAuthorizationUrl());
        }

        private async Task HandleResponseAsync(Uri url)
        {
            this.webBrowser1.Invoke(new Action(() =>
            {
                this.webBrowser1.Stop();
                this.webBrowser1.Visible = false;
            }));

            this.Invoke(new Action(() =>
            {
                this.Text = "Logging in...";
            }));

            var query = HttpUtility.ParseQueryString(url.Query);
            var code = query["code"];
            var error = query["error"];

            if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(error))
            {
                error = "no auth code in response";
            }

            if (!string.IsNullOrEmpty(error))
            {
                Application.Exit();
                Console.WriteLine($"ERROR: {error}");
                Console.Error.WriteLine(error);
                return;
            }



            using (var httpClient = new HttpClient())
            {
                var tokenRequest = new HttpRequestMessage(HttpMethod.Post, this._options.TokenUrl)
                {
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["client_id"] = this._options.ClientId,
                        ["redirect_uri"] = this._options.RedirectUrl,
                        ["scope"] = this._options.Scope,
                        ["grant_type"] = "authorization_code",
                        ["client_secret"] = this._options.ClientSecret,
                        ["code"] = code,
                    })
                };

                var response = await httpClient.SendAsync(tokenRequest);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    this.Invoke(new Action(() =>
                    {
                        Application.Exit();
                        Console.WriteLine(result);
                    }));
                }
            }
        }

        private string CreateAuthorizationUrl()
        {
            return $"{this._options.AuthorizationUrl}?client_id={this._options.ClientId}" +
                $"&response_type=code" +
                $"&redirect_uri={this._options.RedirectUrl}" +
                $"&response_mode=query" +
                $"&scope={this._options.Scope}" +
                $"&state=12345";
        }
    }
}
