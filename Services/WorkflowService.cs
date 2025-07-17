// This file has the service for workflows
using WorkflowEngine.Models; // get the models
using WorkflowEngine.Storage; // get the storage
using System; // for stuff like Exception
// using System.Linq; // for lists and stuff (not needed now)

namespace WorkflowEngine.Services
{
    // This is the service that does workflow things
    public class WorkflowService {
        // These are the repositories for definitions and instances
        private readonly IRepository<WorkflowDefinition> _definitions;
        private readonly IRepository<WorkflowInstance> _instances;

        // This is the constructor, it gets the repos
        public WorkflowService(IRepository<WorkflowDefinition> definitions, IRepository<WorkflowInstance> instances) {
            _definitions = definitions;
            _instances = instances;
        }

        // This makes a new workflow definition
        public bool CreateWorkflowDefinition(WorkflowDefinition def, out string error) {
            if (def.Id == null || def.Id.Trim() == "")
            {
                def.Id = Guid.NewGuid().ToString(); // make a new id if none
            }

            if (_definitions.Get(def.Id) != null) {
                error = "Workflow with id '" + def.Id + "' already exists.";
                return false;
            }

            if (!IsValidDefinition(def, out error))
            {
                return false;
            }

            _definitions.Add(def);
            error = "";
            return true;
        }

        // This checks if a workflow definition is ok
        public bool IsValidDefinition(WorkflowDefinition def, out string error) {
            var stateIds = new System.Collections.Generic.List<string>();
            for (int i = 0; i < def.States.Count; i++)
            {
                stateIds.Add(def.States[i].Id);
            }
            if (def.States == null || def.States.Count < 2) {
                error = "Workflow must have at least two states.";
                return false;
            }
            if (def.Actions == null || def.Actions.Count < 1) {
                error = "Workflow must have at least one action.";
                return false;
            }
            int initialCount = 0;
            for (int i = 0; i < def.States.Count; i++)
            {
                if (def.States[i].IsInitial) initialCount++;
            }
            if (initialCount != 1) {
                error = "Workflow must have exactly one initial state.";
                return false;
            }
            var stateIdCounts = new System.Collections.Generic.Dictionary<string, int>();
            for (int i = 0; i < def.States.Count; i++)
            {
                string id = def.States[i].Id;
                if (!stateIdCounts.ContainsKey(id)) stateIdCounts[id] = 0;
                stateIdCounts[id]++;
            }
            foreach (var kv in stateIdCounts)
            {
                if (kv.Value > 1)
                {
                    error = "Duplicate state IDs found.";
                    return false;
                }
            }
            for (int i = 0; i < def.Actions.Count; i++)
            {
                var a = def.Actions[i];
                bool foundToState = false;
                for (int j = 0; j < stateIds.Count; j++)
                {
                    if (stateIds[j] == a.ToState) foundToState = true;
                }
                if (!foundToState)
                {
                    error = "Action refers to unknown state.";
                    return false;
                }
                for (int k = 0; k < a.FromStates.Count; k++)
                {
                    bool foundFromState = false;
                    for (int j = 0; j < stateIds.Count; j++)
                    {
                        if (stateIds[j] == a.FromStates[k]) foundFromState = true;
                    }
                    if (!foundFromState)
                    {
                        error = "Action refers to unknown state.";
                        return false;
                    }
                }
            }
            error = "";
            return true;
        }

        // This starts a workflow instance
        public WorkflowInstance StartInstance(string definitionId) {
            var def = _definitions.Get(definitionId);
            if (def == null)
            {
                throw new Exception("Definition not found.");
            }
            State init = null;
            for (int i = 0; i < def.States.Count; i++)
            {
                if (def.States[i].IsInitial) init = def.States[i];
            }
            if (init == null)
            {
                throw new Exception("No initial state found.");
            }
            var instance = new WorkflowInstance {
                Id = Guid.NewGuid().ToString(),
                DefinitionId = definitionId,
                CurrentState = init.Id
            };
            _instances.Add(instance);
            return instance;
        }

        // This does an action on a workflow instance
        public WorkflowInstance ExecuteAction(string instanceId, string actionId) {
            var instance = _instances.Get(instanceId);
            if (instance == null)
            {
                throw new Exception("Instance not found.");
            }
            var def = _definitions.Get(instance.DefinitionId);
            if (def == null)
            {
                throw new Exception("Definition not found.");
            }
            WorkflowEngine.Models.Action action = null;
            for (int i = 0; i < def.Actions.Count; i++)
            {
                if (def.Actions[i].Id == actionId) action = def.Actions[i];
            }
            if (action == null)
            {
                throw new Exception("Action not found in workflow definition.");
            }
            if (!action.Enabled)
            {
                throw new Exception("Action is disabled.");
            }
            bool found = false;
            for (int i = 0; i < action.FromStates.Count; i++)
            {
                if (action.FromStates[i] == instance.CurrentState) found = true;
            }
            if (!found)
            {
                throw new Exception("Invalid transition: current state not in action's source states.");
            }
            State current = null;
            for (int i = 0; i < def.States.Count; i++)
            {
                if (def.States[i].Id == instance.CurrentState) current = def.States[i];
            }
            if (current != null && current.IsFinal)
            {
                throw new Exception("No actions allowed from final state.");
            }
            instance.CurrentState = action.ToState;
            var entry = new HistoryEntry();
            entry.ActionId = action.Id;
            entry.Timestamp = DateTime.UtcNow;
            instance.History.Add(entry);
            return instance;
        }

        // This adds a state to a workflow
        public bool AddStateToWorkflow(string workflowId, State state, out string error) {
            var def = _definitions.Get(workflowId);
            if (def == null) {
                error = "Workflow not found.";
                return false;
            }
            for (int i = 0; i < def.States.Count; i++)
            {
                if (def.States[i].Id == state.Id)
                {
                    error = "State with this ID already exists.";
                    return false;
                }
            }
            if (state.IsInitial)
            {
                for (int i = 0; i < def.States.Count; i++)
                {
                    if (def.States[i].IsInitial)
                    {
                        error = "There is already an initial state in this workflow. Only one initial state is allowed.";
                        return false;
                    }
                }
            }
            def.States.Add(state);
            if (!IsValidDefinition(def, out error)) {
                def.States.Remove(state);
                return false;
            }
            _definitions.Add(def);
            return true;
        }

        // This adds an action to a workflow
        public bool AddActionToWorkflow(string workflowId, WorkflowEngine.Models.Action action, out string error) {
            var def = _definitions.Get(workflowId);
            if (def == null) {
                error = "Workflow not found.";
                return false;
            }
            for (int i = 0; i < def.Actions.Count; i++)
            {
                if (def.Actions[i].Id == action.Id)
                {
                    error = "Action with this ID already exists.";
                    return false;
                }
            }
            def.Actions.Add(action);
            if (!IsValidDefinition(def, out error)) {
                def.Actions.Remove(action);
                return false;
            }
            _definitions.Add(def);
            return true;
        }
    }
} 