// This is the main file where the app starts
using WorkflowEngine.Models; // Import models
using WorkflowEngine.Storage; // Import storage stuff
using WorkflowEngine.Services; // Import services

// This is how you start the web app
var builder = WebApplication.CreateBuilder(args); // make a builder

// Register repositories and services so we can use them later
builder.Services.AddSingleton<IRepository<MachineDefinition>, InMemoryRepository<MachineDefinition>>(); // machine def repo
builder.Services.AddSingleton<IRepository<MachineInstance>, InMemoryRepository<MachineInstance>>(); // machine instance repo
builder.Services.AddSingleton<MachineService>(); // machine service

var app = builder.Build(); // build the app

// This is for creating a machine definition
app.MapPost("/machine-definitions", (MachineDefinition def, MachineService service) => {
    Console.WriteLine("POST /machine-definitions endpoint hit");
    if (def == null)
    {
        Console.WriteLine("Request body is missing");
        return Results.BadRequest("Request body is missing");
    }
    string errorMsg;
    bool ok = service.CreateMachineDefinition(def, out errorMsg);
    if (!ok)
    {
        Console.WriteLine("Failed to create machine: " + errorMsg);
        return Results.BadRequest(errorMsg);
    }
    return Results.Ok(def);
});

// This gets a machine definition by id
app.MapGet("/machine-definitions/{id}", (string id, IRepository<MachineDefinition> repo) => {
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
        Console.WriteLine("Machine not found: " + id);
        return Results.NotFound();
    }
});

// This gets all machine definitions
app.MapGet("/machine-definitions", (IRepository<MachineDefinition> repo) => {
    Console.WriteLine("GET /machine-definitions endpoint hit");
    var allDefs = repo.GetAll();
    List<MachineDefinition> result = new List<MachineDefinition>();
    for (int i = 0; i < allDefs.Count; i++)
    {
        result.Add(allDefs[i]);
    }
    return Results.Ok(result);
});

// This is for starting a machine instance
app.MapPost("/machine-instances", (string definitionId, MachineService service) => {
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

// This is for doing an action on a machine instance
app.MapPost("/machine-instances/{id}/actions/{actionId}", (string id, string actionId, MachineService service) => {
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

// This gets a machine instance by id
app.MapGet("/machine-instances/{id}", (string id, IRepository<MachineInstance> repo) => {
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

// This gets all machine instances
app.MapGet("/machine-instances", (IRepository<MachineInstance> repo) => {
    var allInst = repo.GetAll();
    List<MachineInstance> result = new List<MachineInstance>();
    for (int i = 0; i < allInst.Count; i++)
    {
        result.Add(allInst[i]);
    }
    return Results.Ok(result);
});

// This gets all states for a machine definition
app.MapGet("/machine-definitions/{id}/states", (string id, IRepository<MachineDefinition> repo) => {
    if (string.IsNullOrEmpty(id))
    {
        Console.WriteLine("Id is required");
        return Results.BadRequest("Id is required");
    }
    var def = repo.Get(id);
    if (def == null)
    {
        Console.WriteLine("Machine not found for states: " + id);
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

// This gets all actions for a machine definition
app.MapGet("/machine-definitions/{id}/actions", (string id, IRepository<MachineDefinition> repo) => {
    if (string.IsNullOrEmpty(id))
    {
        Console.WriteLine("Id is required");
        return Results.BadRequest("Id is required");
    }
    var def = repo.Get(id);
    if (def == null)
    {
        Console.WriteLine("Machine not found for actions: " + id);
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

// This adds a state to a machine definition
app.MapPost("/machine-definitions/{id}/states", (string id, State state, MachineService service) => {
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
    bool ok = service.AddStateToMachine(id, state, out errorMsg);
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

// This adds an action to a machine definition
app.MapPost("/machine-definitions/{id}/actions", (string id, WorkflowEngine.Models.Action action, MachineService service) => {
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
    bool ok = service.AddActionToMachine(id, action, out errorMsg);
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
