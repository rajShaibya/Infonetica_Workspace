// This is the interface for a repository
namespace WorkflowEngine.Storage
{
    // This is an interface for storing and getting things
    public interface IRepository<T> {
        // This adds an item
        void Add(T item);
        // This gets an item by id
        T Get(string id);
        // This gets all the items
        List<T> GetAll();
    }
} 