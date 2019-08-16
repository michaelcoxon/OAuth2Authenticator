namespace OAuth2Authenticator
{
    public interface IResponseHandler
    {
        void Execute(string tokenJson);
    }
}