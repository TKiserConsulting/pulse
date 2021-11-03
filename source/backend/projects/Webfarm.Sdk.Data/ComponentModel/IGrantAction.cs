namespace Webfarm.Sdk.Data.ComponentModel
{
    public interface IGrantAction
    {
        string GrantType { get; set; }

        string GrantAction { get; set; }
    }
}
