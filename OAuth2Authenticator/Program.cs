using CommandLine;
using OAuth2Authenticator.ResponseHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OAuth2Authenticator
{
    static class Program
    {
        private static ApplicationContext _context;
        [STAThread]
        static void Main(string[] args)
        {
            var mainParser = new Parser(options =>
            {
                options.CaseInsensitiveEnumValues = false;
                options.CaseSensitive = false;
                options.IgnoreUnknownArguments = true;
            });

            var rhParser = new Parser(options =>
            {
                options.CaseInsensitiveEnumValues = false;
                options.CaseSensitive = false;
                options.IgnoreUnknownArguments = false;
            });

            mainParser.ParseArguments<CommandLineOptions>(args)
                .WithParsed(opts =>
                {
                    // if successful, lets launch the app

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    IResponseHandler responseHandler = null;
                    switch (opts.ResponseHandler)
                    {
                        case ResponseHandlerEnum.StdOut:
                            rhParser.ParseArguments<StdOutResponseHandler.StdOutCommandLineOptions>(args)
                                .WithParsed(rhOpts => responseHandler = new StdOutResponseHandler(rhOpts))
                                .WithNotParsed(errors =>
                                {
                                    // bad args are bad.
                                    Environment.ExitCode = -1;
                                    Application.Exit();
                                })
                                ;
                            break;

                        case ResponseHandlerEnum.Env:
                            responseHandler = new EnvironmentVariableResponseHandler();
                            break;
                    }

                    _context = new ApplicationContext(new Form1(opts, responseHandler));
                    _context.ThreadExit += Application_ApplicationExit;

                    Application.Run(_context);
                })
                .WithNotParsed(errors =>
                {
                    // bad args are bad.
                    Environment.ExitCode = -1;
                    Application.Exit();
                });
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
#if DEBUG
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
#endif
        }
    }
}
