// This file has the State and Action classes for the workflow
namespace WorkflowEngine.Models
{
    // This is a state in the workflow
    public class State {
        // The id for the state
        public string Id { get; set; } = null!;
        // The name of the state
        public string Name { get; set; } = null!;
        // What this state means
        public string Description { get; set; } = string.Empty;
        // Is this the first state?
        public bool IsInitial { get; set; }
        // Is this the last state?
        public bool IsFinal { get; set; }
        // Can you use this state?
        public bool Enabled { get; set; } = true;
    }

    // This is an action you can do in the workflow
    public class Action {
        // The id for the action
        public string Id { get; set; } = null!;
        // The name of the action
        public string Name { get; set; } = null!;
        // Is this action allowed?
        public bool Enabled { get; set; } = true;
        // What states can you do this action from?
        public List<string> FromStates { get; set; } = new List<string>();
        // Where does this action go to?
        public string ToState { get; set; } = null!;
        // What does this action do?
        public string Description { get; set; } = string.Empty;
    }
} 