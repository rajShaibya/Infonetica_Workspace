# Machine Engine Infonetica Assignment

This project is a basic machine engine built with C# and ASP.NET Core. It lets you define machines, states, and actions. You can choose states and define actions that map from one state to another. NOTE: You can have only one initial state.

---

## Project Structure

```
Infonetica_Workflow/
│   Program.cs
│   WorkflowEngine.csproj
│   appsettings.json
│   appsettings.Development.json
│   README.md
│
├── Models/
│     ├── StateActionModels.cs
│     └── WorkflowModels.cs
│
├── Services/
│     └── WorkflowService.cs
│
└── Storage/
      ├── InMemoryRepository.cs
      └── IRepository.cs
```

---

## How to Clone and Run

1. **Clone the repository:**
   ```bash
   git clone https://github.com/rajShaibya/Infonetica_Workspace
   cd Infonetica_Workspace
   ```
2. **Build the project (optional):**
   ```bash
   dotnet build
   ```
3. **Run the API:**
   ```bash
   dotnet run
   ```
   The API will start and listen on:
   `http://localhost:5000`

---

## API Endpoints

- `POST   /machine-definitions` – Create a new machine
- `GET    /machine-definitions` – List all machines
- `GET    /machine-definitions/{id}` – Get a machine by ID
- `GET    /machine-definitions/{id}/states` – List states for a machine
- `GET    /machine-definitions/{id}/actions` – List actions for a machine
- `POST   /machine-definitions/{id}/states` – Add a state to a machine
- `POST   /machine-definitions/{id}/actions` – Add an action to a machine
- `POST   /machine-instances?definitionId={id}` – Start a new machine instance
- `GET    /machine-instances` – List all machine instances
- `GET    /machine-instances/{id}` – Get instance status and history
- `POST   /machine-instances/{id}/actions/{actionId}` – Execute an action on an instance

---

## Sample JSON and Usage

### Create Machine Definition
```json
{
  "id": "machine1",
  "states": [
    { "id": "idle", "name": "Idle", "isInitial": true, "isFinal": false, "enabled": true, "description": "Initial State"},
    { "id": "running", "name": "Running", "isInitial": false, "isFinal": false, "enabled": true },
    { "id": "stopped", "name": "Stopped", "isInitial": false, "isFinal": true, "enabled": true }
  ],
  "actions": [
    { "id": "start", "name": "Start Machine", "enabled": true, "fromStates": ["idle"], "toState": "running" },
    { "id": "stop", "name": "Stop Machine", "enabled": true, "fromStates": ["running"], "toState": "stopped" }
  ]
}
```

### Start Machine Instance
```
POST /machine-instances?definitionId=machine1
```

### Execute Action
```
POST /machine-instances/{instanceId}/actions/start
```

### State and Action with Description
```json
{
  "id": "idle",
  "name": "Idle",
  "description": "This is the idle state.",
  "isInitial": true,
  "isFinal": false,
  "enabled": true
}
```

```json
{
  "id": "start",
  "name": "Start Machine",
  "description": "Start the machine.",
  "enabled": true,
  "fromStates": ["idle"],
  "toState": "running"
}
```

### Add State to Machine
```
POST /machine-definitions/{id}/states
```
Body:
```json
{
  "id": "newstate",
  "name": "New State",
  "description": "A new state added to the machine.",
  "isInitial": false,
  "isFinal": false,
  "enabled": true
}
```

### Add Action to Machine
```
POST /machine-definitions/{id}/actions
```
Body:
```json
{
  "id": "newaction",
  "name": "New Action",
  "description": "A new action added to the machine.",
  "enabled": true,
  "fromStates": ["someStateId"],
  "toState": "anotherStateId"
}
```

---

## Notes
- **All data is in-memory only.** When the app stops, all machines and instances are lost.
- **Runs on port 5000** by default (see console output for confirmation).
- **Tested with Postman.**
- Models, storage, and endpoints are clearly separated for maintainability.

