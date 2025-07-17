// This is a simple in-memory repository for storing stuff
using System.Collections.Generic; // for Dictionary and List
// using System.Linq; // for ToList (not needed now)

namespace WorkflowEngine.Storage
{
    // This is a class for storing things in memory
    public class InMemoryRepository<T> : IRepository<T> where T : class {
        // This is where we keep the items
        private Dictionary<string, T> _store = new Dictionary<string, T>();

        // This adds an item to the store
        public void Add(T item) {
            // Get the Id property using reflection (kind of advanced)
            var idProp = item.GetType().GetProperty("Id");
            if (idProp == null)
            {
                throw new System.Exception("No Id property found on type " + typeof(T).Name);
            }
            var idValue = idProp.GetValue(item) as string;
            if (string.IsNullOrEmpty(idValue))
            {
                throw new System.Exception("Id property is null or empty for type " + typeof(T).Name);
            }
            _store[idValue] = item; // put it in the dictionary
        }

        // This gets an item by id
        public T Get(string id) {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            if (_store.ContainsKey(id))
            {
                return _store[id];
            }
            else
            {
                return null;
            }
        }

        // This gets all the items
        public List<T> GetAll() {
            List<T> result = new List<T>();
            foreach (var value in _store.Values)
            {
                result.Add(value);
            }
            return result;
        }
    }
} 