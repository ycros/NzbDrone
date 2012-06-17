namespace Exceptron.Driver
{
    internal interface IRestClient
    {
        TResponse Put<TResponse>(string url, object report) where TResponse : class, new();
    }
}