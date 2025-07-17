// This file has the MachineDefinition, MachineInstance, and HistoryEntry classes
namespace WorkflowEngine.Models
{
    // This is the definition of a machine
    public class MachineDefinition {
        // The id for the machine
        public string Id { get; set; } = null!;
        // All the states in the machine
        public List<State> States { get; set; } = new List<State>();
        // All the actions in the machine
        public List<Action> Actions { get; set; } = new List<Action>();
    }

    // This is an instance of a machine
    public class MachineInstance {
        // The id for the instance
        public string Id { get; set; } = null!;
        // The id of the machine definition
        public string DefinitionId { get; set; } = null!;
        // The current state of the instance
        public string CurrentState { get; set; } = null!;
        // The history of what happened
        public List<HistoryEntry> History { get; set; } = new List<HistoryEntry>();
    }

    // This is for keeping track of what actions happened
    public class HistoryEntry {
        // The id of the action that was done
        public string ActionId { get; set; } = null!;
        // When did it happen?
        public DateTime Timestamp { get; set; }
    }
} 