namespace Lsp.Models
{
    public interface IExecuteCommandOptions
    {
        Container<string> Commands { get; set; }
    }
}