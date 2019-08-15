## OAuth2Authenticator

A command line tool that lets you authenticate via OAuth2. The authenticator will pop a window and allow the user to login. Access tokens are printed to STD_OUT as JSON.

## Usage

```sh
oauth2 --authorize_uri https://login.microsoftonline.com/common/oauth2/v2.0/authorize
       --redirect_uri https://literallyanything
       --token_uri https://login.microsoftonline.com/common/oauth2/v2.0/token
       --client_id myclientid
       --client_secret MySuPeRsEcReT
       --scope "openid offline_access user.read"
```

This command will output one of the following:

1. On a successful authentication, the utility will dump the JSON of the token URL response to the console STDOUT with an error code of 0.
2. On a failed authentication, the utility will set the error code to -1 and dump the error response to both STDOUT and STDERR.

## Compile

The code will compile to the project name for DEBUG builds and to `oauth2` in RELEASE builds.

To compile, open the project in Visual Studio and build it. There really is nothing to it.

## Contribute

Yes.. contribute! just issue a pull request and I will do a code review and either merge it in or offer changes. Nothing too crazy round these parts.