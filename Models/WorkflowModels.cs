// This file has the WorkflowDefinition, WorkflowInstance, and HistoryEntry classes
namespace WorkflowEngine.Models
{
    // This is the definition of a workflow
    public class WorkflowDefinition {
        // The id for the workflow
        public string Id { get; set; } = null!;
        // All the states in the workflow
        public List<State> States { get; set; } = new List<State>();
        // All the actions in the workflow
        public List<Action> Actions { get; set; } = new List<Action>();
    }

    // This is an instance of a workflow
    public class WorkflowInstance {
        // The id for the instance
        public string Id { get; set; } = null!;
        // The id of the workflow definition
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