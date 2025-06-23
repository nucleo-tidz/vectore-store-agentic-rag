namespace infrastructure.Agents
{
    public interface IProjectAgent
    {
        Task<string> Execute(string message);
    }
}
