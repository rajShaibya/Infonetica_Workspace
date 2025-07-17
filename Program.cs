// This is the main file where the app starts
using WorkflowEngine.Models; // Import models
using WorkflowEngine.Storage; // Import storage stuff
using WorkflowEngine.Services; // Import services

// This is how you start the web app
var builder = WebApplication.CreateBuilder(args); // make a builder

// Register repositories and services so we can use them later
builder.Services.AddSingleton<IRepository<WorkflowDefinition>, InMemoryRepository<WorkflowDefinition>>(); // workflow def repo
builder.Services.AddSingleton<IRepository<WorkflowInstance>, InMemoryRepository<WorkflowInstance>>(); // workflow instance repo
builder.Services.AddSingleton<WorkflowService>(); // workflow service

var app = builder.Build(); // build the app

// This is for creating a workflow definition
// Example fetch call from JavaScript:
// fetch('http://localhost:5000/workflow-definitions', {
//   method: 'POST',
//   headers: { 'Content-Type': 'application/json' },
//   body: JSON.stringify({ id: 'work1', states: [], actions: [] })
// }).then(res => res.json()).then(console.log);
app.MapPost("/workflow-definitions", (WorkflowDefinition def, WorkflowService service) => {
    Console.WriteLine("POST /workflow-definitions endpoint hit");
    if (def == null)
    {
        Console.WriteLine("Request body is missing");
        return Results.BadRequest("Request body is missing");
    }
    string errorMsg;
    bool ok = service.CreateWorkflowDefinition(def, out errorMsg);
    if (!ok)
    {
        Console.WriteLine("Failed to create workflow: " + errorMsg);
        return Results.BadRequest(errorMsg);
    }
    return Results.Ok(def);
});

// This gets a workflow definition by id
app.MapGet("/workflow-definitions/{id}", (string id, IRepository<WorkflowDefinition> repo) => {
    if (string.IsNullOrEmpty(id))
    {
        Console.WriteLine("Id is required");
        return Results.BadRequest("Id is required");
    }
    var found = repo.Get(id);
    if (found != null)
    {
        return Results.Ok(found);
    }
    else
    {
        Console.WriteLine("Workflow not found: " + id);
        return Results.NotFound();
    }
});

// This gets all workflow definitions
app.MapGet("/workflow-definitions", (IRepository<WorkflowDefinition> repo) => {
    Console.WriteLine("GET /workflow-definitions endpoint hit");
    var allDefs = repo.GetAll();
    List<WorkflowDefinition> result = new List<WorkflowDefinition>();
    for (int i = 0; i < allDefs.Count; i++)
    {
        result.Add(allDefs[i]);
    }
    return Results.Ok(result);
});

// This is for starting a workflow instance
app.MapPost("/workflow-instances", (string definitionId, WorkflowService service) => {
    if (string.IsNullOrEmpty(definitionId))
    {
        Console.WriteLine("definitionId is required");
        return Results.BadRequest("definitionId is required");
    }
    try
    {
        var instance = service.StartInstance(definitionId);
        return Results.Ok(instance);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Failed to start instance: " + ex.Message);
        return Results.BadRequest(ex.Message);
    }
});

// This is for doing an action on a workflow instance
app.MapPost("/workflow-instances/{id}/actions/{actionId}", (string id, string actionId, WorkflowService service) => {
    if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(actionId))
    {
        Console.WriteLine("id and actionId are required");
        return Results.BadRequest("id and actionId are required");
    }
    try
    {
        var updatedInstance = service.ExecuteAction(id, actionId);
        return Results.Ok(updatedInstance);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Failed to execute action: " + ex.Message);
        return Results.BadRequest(ex.Message);
    }
});

// This gets a workflow instance by id
app.MapGet("/workflow-instances/{id}", (string id, IRepository<WorkflowInstance> repo) => {
    if (string.IsNullOrEmpty(id))
    {
        Console.WriteLine("Id is required");
        return Results.BadRequest("Id is required");
    }
    var inst = repo.Get(id);
    if (inst != null)
    {
        return Results.Ok(inst);
    }
    else
    {
        Console.WriteLine("Instance not found: " + id);
        return Results.NotFound();
    }
});

// This gets all workflow instances
app.MapGet("/workflow-instances", (IRepository<WorkflowInstance> repo) => {
    var allInst = repo.GetAll();
    List<WorkflowInstance> result = new List<WorkflowInstance>();
    for (int i = 0; i < allInst.Count; i++)
    {
        result.Add(allInst[i]);
    }
    return Results.Ok(result);
});

// This gets all states for a workflow definition
app.MapGet("/workflow-definitions/{id}/states", (string id, IRepository<WorkflowDefinition> repo) => {
    if (string.IsNullOrEmpty(id))
    {
        Console.WriteLine("Id is required");
        return Results.BadRequest("Id is required");
    }
    var def = repo.Get(id);
    if (def == null)
    {
        Console.WriteLine("Workflow not found for states: " + id);
        return Results.NotFound();
    }
    else
    {
        List<State> result = new List<State>();
        for (int i = 0; i < def.States.Count; i++)
        {
            result.Add(def.States[i]);
        }
        return Results.Ok(result);
    }
});

// This gets all actions for a workflow definition
app.MapGet("/workflow-definitions/{id}/actions", (string id, IRepository<WorkflowDefinition> repo) => {
    if (string.IsNullOrEmpty(id))
    {
        Console.WriteLine("Id is required");
        return Results.BadRequest("Id is required");
    }
    var def = repo.Get(id);
    if (def == null)
    {
        Console.WriteLine("Workflow not found for actions: " + id);
        return Results.NotFound();
    }
    else
    {
        List<WorkflowEngine.Models.Action> result = new List<WorkflowEngine.Models.Action>();
        for (int i = 0; i < def.Actions.Count; i++)
        {
            result.Add(def.Actions[i]);
        }
        return Results.Ok(result);
    }
});

// This adds a state to a workflow definition
app.MapPost("/workflow-definitions/{id}/states", (string id, State state, WorkflowService service) => {
    if (string.IsNullOrEmpty(id))
    {
        Console.WriteLine("Id is required");
        return Results.BadRequest("Id is required");
    }
    if (state == null)
    {
        Console.WriteLine("State is required");
        return Results.BadRequest("State is required");
    }
    string errorMsg;
    bool ok = service.AddStateToWorkflow(id, state, out errorMsg);
    if (ok)
    {
        return Results.Ok(state);
    }
    else
    {
        Console.WriteLine("Failed to add state: " + errorMsg);
        return Results.BadRequest(errorMsg);
    }
});

// This adds an action to a workflow definition
app.MapPost("/workflow-definitions/{id}/actions", (string id, WorkflowEngine.Models.Action action, WorkflowService service) => {
    if (string.IsNullOrEmpty(id))
    {
        Console.WriteLine("Id is required");
        return Results.BadRequest("Id is required");
    }
    if (action == null)
    {
        Console.WriteLine("Action is required");
        return Results.BadRequest("Action is required");
    }
    string errorMsg;
    bool ok = service.AddActionToWorkflow(id, action, out errorMsg);
    if (ok)
    {
        return Results.Ok(action);
    }
    else
    {
        Console.WriteLine("Failed to add action: " + errorMsg);
        return Results.BadRequest(errorMsg);
    }
});

// This starts the app
app.Run();
