namespace IdentityServer.Traditional.Messaging.Base
{
    public abstract class BaseApiRequest
    {
    }

    public abstract class BaseApiRequest<T> : BaseApiRequest
    {
        public T? ViewModel { get; set; }
    }
}
