using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace OAuth2Authenticator
{
    public partial class Form1 : Form
    {
        private readonly CommandLineOptions _options;
        private readonly IResponseHandler _responseHandler;
        private string _state;

        public Form1(CommandLineOptions options, IResponseHandler responseHandler)
        {
            this._options = options;
            this._responseHandler = responseHandler;

            this.InitializeComponent();

            this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var runOnce = true;

            // use the browser title as the form title.
            this.webBrowser1.DocumentTitleChanged += (s, e2) =>
            {
                this.Invoke(new Action(() =>
                {
                    this.Text = this.webBrowser1.DocumentTitle;
                }));
            };

            // We need to catch this event here cause there is a possiblity
            // of the redirect url not existing. so we check every progress 
            // change to see if it is for our capture point if the progress
            // is completed.
            this.webBrowser1.ProgressChanged += (s, e2) =>
            {
                if (runOnce && e2.CurrentProgress == e2.MaximumProgress && (this.webBrowser1.Url?.ToString().StartsWith(this._options.RedirectUrl, StringComparison.InvariantCultureIgnoreCase) ?? false))
                {
                    runOnce = false;
                    Task.Run(() => this.HandleResponseAsync(this.webBrowser1.Url));
                }
            };

            this._state = Guid.NewGuid().ToString();

            // Let's do this...
            this.webBrowser1.Navigate(this.CreateAuthorizationUrl(this._state));
        }

        private async Task HandleResponseAsync(Uri url)
        {
            // hide the browser - it displays a error page if the redirect url is fake.
            this.webBrowser1.Invoke(new Action(() =>
            {
                this.webBrowser1.Stop();
                this.webBrowser1.Visible = false;
            }));

            // change our title cause the browser sets this to an error.
            this.Invoke(new Action(() =>
            {
                this.Text = "Logging in...";
            }));

            // get our bits for the token request and any errors.
            var query = HttpUtility.ParseQueryString(url.Query);
            var code = query["code"];
            var error = query["error"];
            var state = query["state"];

            //
            // handle the errors
            //

            if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(error))
            {
                error = "no auth code in response";
            }

            if (string.IsNullOrEmpty(state) && string.IsNullOrEmpty(error))
            {
                error = "no state in response";
            }

            if (state != this._state && string.IsNullOrEmpty(error))
            {
                error = "invalid_state";
            }

            if (!string.IsNullOrEmpty(error))
            {
                Console.Error.WriteLine(error);
                Environment.ExitCode = -1;
                Application.Exit();
                return;
            }

            //
            // make a request for the access tokens
            //

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
                        this._responseHandler.Execute(result);
                        Environment.ExitCode = 0;
                        Application.Exit();
                    }));
                }
                else
                {
                    var dump = response.Headers
                        .ToDictionary(h => h.Key, h => h.Value)
                        .SelectMany(i =>
                        {
                            var value = i.Value.ToArray();
                            var keyBit = $"{i.Key.ToUpper()}: ";
                            var resultLines = new List<string>
                            {
                                $"{keyBit}{value[0]}"
                            };
                            if (value.Length > 1)
                            {
                                var padding = string.Empty.PadLeft(keyBit.Length);

                                for (var j = 1; j < value.Length; j++)
                                {
                                    resultLines.Add($"{padding}{value[j]}");
                                }
                            }

                            return resultLines;
                        })
                        ;

                    var message = $"TOKEN_REQUEST_FAILED {response.StatusCode} {response.ReasonPhrase}" + Environment.NewLine +
                                  $"{string.Join(Environment.NewLine, dump)}" + Environment.NewLine +
                                  Environment.NewLine + Environment.NewLine +
                                  await response.Content?.ReadAsStringAsync()
                                  ;

                    Console.Error.WriteLine(message);
                    Environment.ExitCode = -1;
                    Application.Exit();
                }
            }
        }

        private string CreateAuthorizationUrl(string state)
        {
            var builder = new UriBuilder(this._options.AuthorizationUrl);

            var query = HttpUtility.ParseQueryString(builder.Query);

            query["client_id"] = this._options.ClientId;
            query["response_type"] = "code";
            query["redirect_uri"] = this._options.RedirectUrl;
            query["response_mode"] = "query";
            query["scope"] = this._options.Scope;
            query["state"] = state;

            builder.Query = string.Join("&", query.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(query[a])));

            return builder.Uri.ToString();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Console.WriteLine($"ERROR: USER_CANCEL");
                Console.Error.WriteLine("USER_CANCEL");
                Environment.ExitCode = -1;
            }
        }
    }
}
