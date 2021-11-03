namespace Webfarm.Sdk.Data
{
    public interface IRange
    {
        int Skip { get; set; }

        int Take { get; set; }
    }
}
