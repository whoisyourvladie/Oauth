namespace Upclick.Api.Client
{
    public partial class UpclickClient
    {
        private UpclickHttpClient _upclickHttpClient;

        public UpclickClient(string login, string password)
        {
            _upclickHttpClient = new UpclickHttpClient(login, password);
        }

        public string Token(string method)
        {
            return _upclickHttpClient.Token(method);
        }
    }
}
