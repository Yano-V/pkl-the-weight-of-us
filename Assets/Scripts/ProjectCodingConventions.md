# Project Coding Convention
> **Note for AI Coding Agents:** Strictly follow the coding conventions written in this markdown document. DO NOT Modify the `.slnx` and `.csproj` files as Unity should ONLY be the one to recompile, regenerate the files. 

When writing code in this Unity Project, programmers are mandated to follow the conventions of the project.

## Variable Naming

### Private
**Variables**: Private variables have no declaration. Naming convention uses camelCase. Unity serializable variables have a `[SerializeField]` attribute in it except if the variable is chosen to be not Serialized. If the variable is a Non-MonoBehaviour (Not Serializable) class, interface, or not serialized, explicitly declare as private.

**Methods**: Private methods have no declaration. By C# Standards, method names use PascalCase but Unity related methods like `Awake()`, `Start()`, `Update()`, and others explicitly declare private as a way to distinguish Unity methods from other methods.

Private Unity-serialized variable:
```csharp
[SerializeField] PlayerAttackCardPool playerAttackCardPool;
```

Do not declare the access modifier for a private Unity-serialized variable:
```csharp
[SerializeField] private PlayerAttackCardPool playerAttackCardPool;
```

For a non-MonoBehaviour class, interface, or non-serialized variable, explicitly declare it as private:
```csharp
private PlayerAttackCardPool playerAttackCardPool;
```

Normal private method:
```csharp
void AddCard(AttackCardData data)
```

Unity related method:
```csharp
private void OnDestroy()
```

### Protected
Protected variables use camelCase. Unity serializable variables have a `[SerializeField]` attribute in it.

Protected variable:
```csharp
[SerializeField] protected ICameraService iCaS;
```

### Public
Public is mostly used by properties. The project minimize the use of public variables for encapsulation. By C# standards, the project uses PascalCase for public properties.

Public property:
```csharp
public IReadOnlyStats Stats => enemyStats;
```

# Service Locator
Use `GameServices` for service registration and initialization only. When a script needs another service, retrieve it during initialization, store it in a field, and use that reference afterward. Do not use `GameServices` to retrieve a service when calling it during regular behavior.

Resolve the dependency during initialization and use the stored reference:
```csharp
private IDataService dataService;

private void Awake()
{
  dataService = GameServices.Get<IDataService>();
}

void HandleSceneLoaded(Scene loadedScene)
{
  SceneData sceneData = dataService.GetSceneDataByName(loadedScene.name);
}
```

Do not retrieve the service from `GameServices` at the point of use:
```csharp
void HandleSceneLoaded(Scene loadedScene)
{
  SceneData sceneData = GameServices.Get<IDataService>().GetSceneDataByName(loadedScene.name);
}
```

All service variables in the codebase must be abbreviated based on their name with an `i` in the start of the variable (resembling the interface naming convention of C#)
```csharp
private ISceneService iSS;
private ISceneServiceManageable iSSM;
private IUIService iUIS;
private IUIServiceManageable iUISM;
```

If there are conflicting abbreviations in the names of the services, use a short but meaningful abbreviation that preserves the service's actual word pattern. Keep the leading `i` and add the next distinctive lowercase letter(s) from the service name to keep it readable and unique.
```csharp
private IPoolService iPoS;
private IPositionService iPosS;
private IPlayerService iPlS;
```

This keeps the naming pattern consistent while avoiding ambiguous or hard-to-read service variables like `iPsS`.

# Event Listeners
When a script listens to an event that uses an enum, NEVER use a guard clause in the event handler to filter enum values. The script may use other values from that enum for different purposes.

Avoid filtering the enum with an early return:
```csharp
void OnCardEvent(CardEventType eventType)
{
  if (eventType != CardEventType.Drawn)
    return;

  HandleDrawn();
}
```

Handle relevant values explicitly instead:
```csharp
void OnCardEvent(CardEventType eventType)
{
  switch (eventType)
  {
    case CardEventType.Drawn:
      HandleDrawn();
      break;
    case CardEventType.Discarded:
      HandleDiscarded();
      break;
  }
}
```

# Commenting Conventions
Here are some provided guidelines for commenting.

## Priority Tag Guidelines
> **Note for AI Coding Agents:** Always include a priority tag when inserting `TODO` or `[ ]` comments based on user plans.

> **Format Example:** `// Example TODO: <Priority = Low> To-do Example here.`
or placed before the class:
```
// To-do List
// Example [ ] - <Priority = Low/Medium/High/Urgent> Task description
```

### Tag Descriptions
- **`<Priority = Low>`** — Non-urgent, nice-to-have tasks or minor polish that can be addressed during downtime without impacting current progress or tasks that can be done later.
- **`<Priority = Medium>`** — Standard, non-blocking tasks or work dependent on high-priority items; should be addressed once immediate prerequisites are complete.
- **`<Priority = High>`** — Time-sensitive tasks essential for core functionality or imminent milestones; requires attention as soon as current blocking work finishes.
- **`<Priority = Urgent>`** — Critical blockers, active breaking bugs, or immediate action items that take precedence over all other tasks.

### Script Descriptions
Below is the format on how scripts are documented:
```
// ================================================================================= //
//                                                                                   //
// Class Name: Player Mediator                                                       //
// Description: The mediator of the player. It serves as the reference for external  //
//              systems to access the player. It orchestrates all the subcomponents  //
//              based on player context.                                             //
// Author: PsychoLucidia                                                             //
//                                                                                   //
// ================================================================================= //
```

## Sorting Fields, Properties and Methods
### Fields/Variables
- Place all fields/variables starting in the first line inside of the class.
- When fields/variables can use the `[SerializeField]` attribute, organize it by using Unity's `[Header()]` attribute based on its role and category.
- Properties, Fields/Variables, and Methods can be grouped in a `#region`/`#endregion` directive.
  - `#region`/`#endregion` directives are named based on how they are grouped. Here are some the mandatory `#region`/`#endregion` names if they are present in the script:
    - **`Interface Implementation Events`**: When a class implements an interface, the events from the interface are grouped under this name.
    - **`Interface Implementation Properties`**: When a class implements an interface, the properties from the interface are grouped under this name.
    - **`Class Only Properties`**: These are properties that are only accessed to those who directly references a class.
    - **`Interface Implementation Methods`**: When a class implements an interface, the methods from the interface are grouped under this name.
    - **`Unity Methods`**: When a class is a `MonoBehaviour`, all Unity Lifecycle methods (`Awake()`, `OnEnable()`, `Start()`, `Update()`, `FixedUpdate()`, `LateUpdate()`, `OnDisable()`, `OnDestroy()`) goes under this group.
    - **`Public Methods`**: These are methods that can be only accessed when other classes have direct reference to the class.
    - **`Private Methods`**: These are methods that can be only accessed to the class itself.
    - **`Utility Methods`**: Private methods that are meant to reduce code redundancy by giving a class a utility to use.
    - **`Unity Editor Methods`**: These are Unity methods that are meant to run on the Unity Editor itself. (`OnDrawGizmos()`, `OnValidate()`, etc.)
  - Each class may have a unique `#region`/`#endregion` group based on their needs.