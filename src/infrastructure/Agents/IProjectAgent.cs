namespace infrastructure.Agents
{
    public interface IProjectAgent
    {
        Task<string> Execute(string input, string userName, string threadId);
    }
}
