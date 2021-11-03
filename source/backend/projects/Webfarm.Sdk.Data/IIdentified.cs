namespace Webfarm.Sdk.Data
{
    public interface IIdentified<T>
    {
        T Id { get; set; }
    }
}
