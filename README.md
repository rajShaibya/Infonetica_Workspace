# Workflow Engine Infonetica Assignment

This project is a basic workflow engine built with C# and ASP.NET Core. It lets you define workflows, states, and actions.Ypu can choose states and define actions that map from one state to other. NOTE: You can have only one initial state.

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
   git clone <your-repo-url>
   cd Infonetica_Test_Task
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

- `POST   /workflow-definitions` – Create a new workflow
- `GET    /workflow-definitions` – List all workflows
- `GET    /workflow-definitions/{id}` – Get a workflow by ID
- `GET    /workflow-definitions/{id}/states` – List states for a workflow
- `GET    /workflow-definitions/{id}/actions` – List actions for a workflow
- `POST   /workflow-definitions/{id}/states` – Add a state to a workflow
- `POST   /workflow-definitions/{id}/actions` – Add an action to a workflow
- `POST   /workflow-instances?definitionId={id}` – Start a new workflow instance
- `GET    /workflow-instances` – List all workflow instances
- `GET    /workflow-instances/{id}` – Get instance status and history
- `POST   /workflow-instances/{id}/actions/{actionId}` – Execute an action on an instance

---

## Sample JSON and Usage

### Create Workflow Definition
```json
{
  "id": "idea1",
  "states": [
    { "id": "idea", "name": "Idea", "isInitial": true, "isFinal": false, "enabled": true, "description": "Initial State"},
    { "id": "implementation", "name": "Implementation", "isInitial": false, "isFinal": false, "enabled": true },
    { "id": "completed", "name": "Completed", "isInitial": false, "isFinal": true, "enabled": true }
  ],
  "actions": [
    { "id": "start_work", "name": "Start Implementation", "enabled": true, "fromStates": ["idea"], "toState": "implementation" },
    { "id": "finish", "name": "Mark as Completed", "enabled": true, "fromStates": ["implementation"], "toState": "completed" }
  ]
}
```

### Start Workflow Instance
```
POST /workflow-instances?definitionId=work1
```

### Execute Action
```
POST /workflow-instances/{instanceId}/actions/submit
```

### State and Action with Description
```json
{
  "id": "draft",
  "name": "Draft",
  "description": "This is the draft state.",
  "isInitial": true,
  "isFinal": false,
  "enabled": true
}
```

```json
{
  "id": "submit",
  "name": "Submit for Review",
  "description": "Submit the document for review.",
  "enabled": true,
  "fromStates": ["draft"],
  "toState": "review"
}
```

### Add State to Workflow
```
POST /workflow-definitions/{id}/states
```
Body:
```json
{
  "id": "newstate",
  "name": "New State",
  "description": "A new state added to the workflow.",
  "isInitial": false,
  "isFinal": false,
  "enabled": true
}
```

### Add Action to Workflow
```
POST /workflow-definitions/{id}/actions
```
Body:
```json
{
  "id": "newaction",
  "name": "New Action",
  "description": "A new action added to the workflow.",
  "enabled": true,
  "fromStates": ["someStateId"],
  "toState": "anotherStateId"
}
```

---

## Notes
- **All data is in-memory only.** When the app stops, all workflows and instances are lost.
- **Runs on port 5000** by default (see console output for confirmation).
- **Tested with Postman.**
- Models, storage, and endpoints are clearly separated for maintainability.

