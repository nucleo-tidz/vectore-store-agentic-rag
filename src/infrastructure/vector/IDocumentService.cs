namespace infrastructure.vector
{
    public interface IDocumentService
    {
        public  Task SaveAsync(string content);
    }
}
