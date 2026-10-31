namespace MiniSupermarket.WinForms;
public static class SessionManager
{
    public static class ApiClientService
    {
        public static HttpClient Client { get; } = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7158/api/"),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }
}
