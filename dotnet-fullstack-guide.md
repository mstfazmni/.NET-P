# Full-Stack .NET Learning Guide — Boxing App Project

A complete reference for everything covered: C#, .NET, ASP.NET Core, EF Core, SQL, React, Redux, and RTK Query — built through one real project (BoxingApp.Api + boxing-frontend).

---

## Table of Contents
1. [C# Fundamentals](#1-c-fundamentals)
2. [SQL Fundamentals](#2-sql-fundamentals)
3. [ASP.NET Core Web API](#3-aspnet-core-web-api)
4. [Entity Framework Core](#4-entity-framework-core)
5. [React Fundamentals](#5-react-fundamentals)
6. [Redux Fundamentals](#6-redux-fundamentals)
7. [RTK Query — Connecting Frontend to Backend](#7-rtk-query--connecting-frontend-to-backend)
8. [How It All Connects — The Full Request Lifecycle](#8-how-it-all-connects--the-full-request-lifecycle)
9. [Project Structure Reference](#9-project-structure-reference)
10. [What's Left — Production-Readiness Roadmap](#10-whats-left--production-readiness-roadmap)

---

## 1. C# Fundamentals

### Why C# feels different from JavaScript
- **Statically typed**: `int x = 5;` — the type is fixed and checked at compile time, unlike JS where a variable can hold anything.
- **Everything lives in a class** — no floating top-level functions like JS allows.
- **Compiled, not interpreted** — C# compiles to an intermediate language (IL) that .NET's runtime executes. Type errors are caught before the program ever runs.

| JavaScript | C# |
|---|---|
| `let`/`const` | `int`, `string`, `bool`, `var` (still fixed-type) |
| `function foo() {}` | `void Foo() {}` inside a class |
| `=>` arrow functions | `=>` lambda expressions |
| `async/await`, Promises | `async/await`, `Task<T>` |
| `.map()`, `.filter()` | LINQ: `.Select()`, `.Where()` |

### OOP — The Four Pillars (plain-English versions)
Think of a class as a blueprint, an object as something built from it.

- **Encapsulation** — "hide the wiring, expose the buttons." Use `{ get; private set; }` to control what outside code can touch.
- **Inheritance** — "a specialized version of something." `ProAthlete : Athlete` means ProAthlete gets everything Athlete has, plus more, via `: base(name)` in the constructor to delegate shared setup to the parent.
- **Polymorphism** — "same method call, different behavior depending on the actual object." Achieved via `virtual` (base) and `override` (derived) — even when a variable is typed as the base class, the *actual* object's overridden method runs.
- **Abstraction** — "a promise of behavior, not the mechanism." An `interface` (e.g. `ITrainingPlanProvider`) declares a method signature without an implementation — any class that implements it just has to honor the contract. This is the exact pattern behind ASP.NET Core's dependency injection.

```csharp
public class Athlete
{
    public string Name { get; set; }
    public int Wins { get; set; }
    public Athlete(string name) { Name = name; }
    public virtual string GetRecord() => $"{Name}: {Wins} wins";
}

public class ProAthlete : Athlete
{
    public ProAthlete(string name) : base(name) { }
    public override string GetRecord() => $"{Name} (Pro): {Wins} wins";
}
```

### LINQ — querying collections (maps to JS array methods)
- `.Where(x => ...)` = `.filter()`
- `.Select(x => ...)` = `.map()`
- `.FirstOrDefault(x => ...)` = `.find()` (returns null instead of undefined)
- `.OrderByDescending(x => ...)` = sorting
- `.Any(x => ...)` = short-circuits on first match — use this for yes/no checks, not `.Where().ToList()` (that builds a whole list just to check if it's non-empty — wasteful)
- **Deferred execution**: `.Where()`/`.Select()` don't run immediately — they build a query plan that only executes when materialized (`.ToList()`, `foreach`). This is exactly how EF Core turns LINQ into real SQL only when needed.

### async/await and Task
- **`Task`** is C#'s version of a JS `Promise` — a "claim ticket" representing work that will finish later.
- **`Task<T>`** = a claim ticket that will hand back a value of type `T` when redeemed via `await`.
- **`Task`** (no generic) = work that finishes but returns nothing.
- Methods doing async work are named with an `Async` suffix by convention (`GetAthleteAsync`).
- `async` does **not** mean "runs on another thread" — it means "doesn't block while waiting" (same idea as JS's event loop).
- `Main` itself can be `static async Task Main(string[] args)` to allow `await` inside it.

```csharp
public static async Task<string> SimulateFightAsync(string athleteName)
{
    await Task.Delay(2000);
    return $"{athleteName} wins the fight!";
}
```

---

## 2. SQL Fundamentals

### Schema basics
```sql
CREATE TABLE Athletes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    WeightClass NVARCHAR(50),
    Wins INT DEFAULT 0,
    Losses INT DEFAULT 0
);

CREATE TABLE TrainingPlans (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AthleteId INT NOT NULL,
    PlanName NVARCHAR(100),
    FOREIGN KEY (AthleteId) REFERENCES Athletes(Id)
);
```
- `IDENTITY(1,1)` = auto-incrementing, starts at 1. **Never manually insert an Id when this is set** — SQL Server rejects it with an `IDENTITY_INSERT` error.
- `FOREIGN KEY` enforces that `AthleteId` in `TrainingPlans` must correspond to a real row in `Athletes`.

### CRUD operations
```sql
SELECT Name, Wins FROM Athletes WHERE Wins > 5;
INSERT INTO Athletes (Name, WeightClass, Wins, Losses) VALUES ('Canelo', 'Middleweight', 12, 2);
UPDATE Athletes SET Wins = Wins + 1 WHERE Id = 1;
DELETE FROM Athletes WHERE Id = 1;
```
Note: SQL uses **single quotes** for strings, never double quotes.

### JOINs
```sql
SELECT Athletes.Name, TrainingPlans.PlanName
FROM Athletes
LEFT JOIN TrainingPlans ON Athletes.Id = TrainingPlans.AthleteId;
```
- `JOIN` (inner) — only rows with a match on both sides.
- `LEFT JOIN` — all rows from the left table, even without a match (shows `NULL` for the missing side).

### Aggregates and GROUP BY
```sql
SELECT WeightClass, SUM(Wins) AS TotalWins
FROM Athletes
GROUP BY WeightClass
HAVING SUM(Wins) > 10;
```
- Any non-aggregated column in `SELECT` must appear in `GROUP BY`.
- `WHERE` filters rows before grouping; `HAVING` filters groups after aggregation.

---

## 3. ASP.NET Core Web API

### What a controller actually is
**Analogy: a waiter.** It listens for incoming HTTP requests, figures out what's being asked, gets/prepares the answer, and hands back a response. It doesn't do the actual "cooking" (database work) itself.

```csharp
[ApiController]
[Route("api/[controller]")]
public class AthletesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAthletes() => Ok(new List<string> { "Canelo", "Floyd" });
}
```

- **`[ApiController]`** — turns on automatic request validation and JSON formatting.
- **`[Route("api/[controller]")]`** — `[controller]` is replaced with the class name minus "Controller" (case-sensitive but ASP.NET Core routing is case-insensitive in practice) → `AthletesController` becomes `api/Athletes`.
- **`: ControllerBase`** (not `Controller`) — the correct base class for API-only projects (no view rendering needed).
- **`IActionResult`** — an umbrella "box" for any HTTP response outcome: `Ok()` (200), `NotFound()` (404), `BadRequest()` (400), `CreatedAtAction()` (201), `NoContent()` (204).
- **You never call controller methods yourself** — the framework calls them automatically when an incoming request's URL matches your route + HTTP verb.

### Route parameters
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetAthleteById(int id) { ... }
```
The `{id}` in the route must match the parameter name. ASP.NET Core auto-converts the URL segment into a typed parameter (unlike Express, where `req.params.id` is always a raw string).

### Full CRUD pattern
```csharp
[HttpPost]
public async Task<IActionResult> CreateAthlete(CreateAthleteDto dto)
{
    var athlete = new Athlete { Name = dto.Name, WeightClass = dto.WeightClass, Wins = dto.Wins, Losses = dto.Losses };
    _context.Athletes.Add(athlete);
    await _context.SaveChangesAsync();
    return CreatedAtAction(nameof(GetAthleteById), new { id = athlete.Id }, athlete);
}

[HttpPut("{id}")]
public async Task<IActionResult> UpdateAthlete(int id, Athlete updatedAthlete)
{
    var athlete = await _context.Athletes.FindAsync(id);
    if (athlete == null) return NotFound();
    athlete.Name = updatedAthlete.Name;
    athlete.Wins = updatedAthlete.Wins;
    await _context.SaveChangesAsync();
    return NoContent();
}

[HttpDelete("{id}")]
public async Task<IActionResult> DeleteAthlete(int id)
{
    var athlete = await _context.Athletes.FindAsync(id);
    if (athlete == null) return NotFound();
    _context.Athletes.Remove(athlete);
    await _context.SaveChangesAsync();
    return NoContent();
}
```
- `_context.Athletes.Add(...)` stages a change; `SaveChangesAsync()` actually commits it to the database.
- `NoContent()` (204) is the correct response for a successful update/delete with nothing to return.

### DTOs (Data Transfer Objects)
**Why they exist:** your database entity (`Athlete`) and "what a client is allowed to send you" are different concerns. A DTO defines exactly what shape of data is accepted, so a client can never set fields they shouldn't control (like `Id`, which the database generates).
```csharp
public class CreateAthleteDto
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; }
    [Required]
    public string WeightClass { get; set; }
    [Range(0, int.MaxValue)]
    public int Wins { get; set; }
    [Range(0, int.MaxValue)]
    public int Losses { get; set; }
}
```
`[ApiController]` automatically validates these attributes before your method body even runs — invalid data gets an automatic `400 Bad Request`.

### CORS
Browsers block JS from calling an API on a different origin (different port counts as different origin) by default, for security. Fix:
```csharp
builder.Services.AddCors(options => options.AddPolicy("AllowReactApp", policy =>
    policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));
// after app = builder.Build():
app.UseCors("AllowReactApp");
```

---

## 4. Entity Framework Core

**What it is:** an ORM (Object-Relational Mapper) — same category as Prisma/Sequelize/TypeORM. Lets you write C#/LINQ instead of raw SQL; EF Core translates behind the scenes.

### DbContext — your connection point to the database
```csharp
public class BoxingAppContext : DbContext
{
    public BoxingAppContext(DbContextOptions<BoxingAppContext> options) : base(options) { }
    public DbSet<Athlete> Athletes { get; set; }
    public DbSet<TrainingPlan> TrainingPlans { get; set; }
}
```
- EF Core only knows about tables represented by a `DbSet` property — nothing more, nothing less.
- A property named exactly `Id` is auto-recognized as the primary key, by convention.
- Navigation properties (e.g. `public Athlete Athlete { get; set; }` on `TrainingPlan`) represent relationships in C# terms — EF Core's version of a `FOREIGN KEY`.

### Registering it
```csharp
builder.Services.AddDbContext<BoxingAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### Using it in a controller (dependency injection)
```csharp
private readonly BoxingAppContext _context;
public AthletesController(BoxingAppContext context) { _context = context; }
```
The framework automatically creates and hands over the context — the controller never creates it itself. This makes testing possible later (swap in a fake context) and centralizes configuration.

### Migrations
**The problem they solve:** keeping your C# model classes and the actual database schema in sync, without hand-writing `ALTER TABLE` statements.
```bash
dotnet tool install --global dotnet-ef   # one-time CLI install
dotnet ef migrations add InitialCreate    # generates the "plan" (C# file), doesn't touch DB yet
dotnet ef database update                 # actually applies it
```
Each migration is incremental and named — like commit history for your schema.

### Repository pattern (first step toward Clean Architecture)
Pulling database logic out of the controller into its own class:
```csharp
public class AthleteRepository
{
    private readonly BoxingAppContext _context;
    public AthleteRepository(BoxingAppContext context) { _context = context; }
    public async Task<List<Athlete>> GetAllAsync() => await _context.Athletes.ToListAsync();
}
```
**Why:** the controller no longer needs to know *how* data is fetched — only *that* it can ask for it. If the database technology changed, only this class would need to change.

---

## 5. React Fundamentals

- **Components** — functions returning JSX.
- **Props** — data passed into a component, read-only from the child's side.
- **State (`useState`)** — data that persists across re-renders and triggers a re-render when changed via its setter function. Never mutate state directly.
- **`useEffect`** — runs side effects tied to a component's lifecycle (e.g., fetching data when a value changes).
- **Controlled inputs** — an `<input>` whose `value` is driven by React state, updated via `onChange`. Guarantees your JS variable always matches what's on screen.
- **Keys in lists** — React needs a stable, unique `key` per item to track identity across re-renders (never use array index if the list can reorder/change).
- **Lifting state up** — when siblings need the same data, move it to their shared parent and pass down via props. This is the exact pain point that motivates Redux once an app grows past a few components.
- **Every render of a component gets its own independent state**, even for the same component function reused multiple times.

---

## 6. Redux Fundamentals

### The problem it solves
Passing data down through many layers of components that don't use it themselves ("prop drilling") gets unmanageable as an app grows. Redux lets any component talk directly to one central store, no matter how deep it is in the tree.

### The three core pieces
- **Store** — one plain object holding shared app state, living outside any component.
- **Action** — a plain object describing "something happened" (`{ type: 'athlete/winRecorded', payload: 'Canelo' }`). Doesn't do anything by itself — just a note.
- **Reducer** — a function that receives current state + an action, and returns the new state. The *only* place allowed to actually change the data.

### Modern Redux with RTK (`createSlice`)
```javascript
import { createSlice } from '@reduxjs/toolkit';

const athletesSlice = createSlice({
  name: 'athletes',
  initialState: [{ name: 'Canelo', wins: 0 }],
  reducers: {
    winRecorded: (state, action) => {
      const athlete = state.find(a => a.name === action.payload);
      athlete.wins += 1; // looks like mutation, but RTK's Immer makes this safe INSIDE a slice only
    },
  },
});

export const { winRecorded } = athletesSlice.actions;
export default athletesSlice.reducer;
```
A **slice** is a blueprint: starting data + every allowed way to change it, bundled together — not the live data itself. The live data actually lives inside the reducer once wired into the store.

### Store setup
```javascript
import { configureStore } from '@reduxjs/toolkit';
import athletesReducer from './athletesSlice';

export const store = configureStore({ reducer: { athletes: athletesReducer } });
```
`athletes: athletesReducer` means `state.athletes` in components is managed by this reducer.

### Using it in components
```jsx
import { useSelector, useDispatch } from 'react-redux';
import { winRecorded } from './athletesSlice';

function AthleteCard({ name }) {
  const wins = useSelector(state => state.athletes.find(a => a.name === name).wins);
  const dispatch = useDispatch();
  return <button onClick={() => dispatch(winRecorded(name))}>Record Win</button>;
}
```
- `useSelector` reads a slice of state and auto re-renders the component when it changes.
- `dispatch` is the *only* door into the store — you never modify it directly.
- Wrap the whole app in `<Provider store={store}>` (in `main.jsx`) so any component can reach the store.

---

## 7. RTK Query — Connecting Frontend to Backend

**What it replaces:** manually writing `fetch()` + loading state + error state + Redux boilerplate every time you need server data.

```javascript
import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const apiSlice = createApi({
  reducerPath: 'api',
  baseQuery: fetchBaseQuery({ baseUrl: 'http://localhost:5213/api' }),
  tagTypes: ['Athlete'],
  endpoints: (builder) => ({
    getAthletes: builder.query({
      query: () => '/Athletes',
      providesTags: ['Athlete'],
    }),
    createAthlete: builder.mutation({
      query: (newAthlete) => ({ url: '/Athletes', method: 'POST', body: newAthlete }),
      invalidatesTags: ['Athlete'],
    }),
    updateAthlete: builder.mutation({
      query: ({ id, ...updates }) => ({ url: `/Athletes/${id}`, method: 'PUT', body: updates }),
      invalidatesTags: ['Athlete'],
    }),
    deleteAthlete: builder.mutation({
      query: (id) => ({ url: `/Athletes/${id}`, method: 'DELETE' }),
      invalidatesTags: ['Athlete'],
    }),
  }),
});

export const { useGetAthletesQuery, useCreateAthleteMutation, useUpdateAthleteMutation, useDeleteAthleteMutation } = apiSlice;
```

Register in the store (needs extra middleware, unlike a plain slice):
```javascript
export const store = configureStore({
  reducer: { athletes: athletesReducer, [apiSlice.reducerPath]: apiSlice.reducer },
  middleware: (getDefaultMiddleware) => getDefaultMiddleware().concat(apiSlice.middleware),
});
```

### Key concepts
- **`query` vs `mutation`**: a query reads data (GET) and auto-runs when a component mounts; a mutation changes data (POST/PUT/DELETE) and only runs when you explicitly call it.
- **Auto-generated hooks**: `use<EndpointName>Query` / `use<EndpointName>Mutation` — never written by hand.
- **Query hooks return an object**: `const { data, isLoading, error } = useGetAthletesQuery();`
- **Mutation hooks return an array**: `const [deleteAthlete] = useDeleteAthleteMutation();` — first item is the trigger function you call yourself. (Forgetting the `()` when calling the hook causes a "not iterable" error.)
- **Tags (`providesTags`/`invalidatesTags`)**: solves RTK Query's caching problem. A query "provides" a tag; a mutation "invalidates" it. When a mutation succeeds, RTK Query automatically re-fetches any query providing that tag — your UI updates itself with zero manual refresh code.

### Example usage
```jsx
function AthleteListFromApi() {
  const { data: athletes, isLoading, error } = useGetAthletesQuery();
  const [deleteAthlete] = useDeleteAthleteMutation();

  if (isLoading) return <p>Loading...</p>;
  if (error) return <p>Something went wrong.</p>;

  return athletes.map(a => (
    <div key={a.id}>
      {a.name} — {a.wins} wins
      <button onClick={() => deleteAthlete(a.id)}>Delete</button>
    </div>
  ));
}
```

---

## 8. How It All Connects — The Full Request Lifecycle

Example: clicking "Delete" on an athlete card in the browser.

1. **React** — `onClick` calls `deleteAthlete(athlete.id)`.
2. **RTK Query** — sends `DELETE http://localhost:5213/api/Athletes/{id}` over HTTP.
3. **CORS** — the browser checks the backend allows requests from `localhost:5173`; it does, because of the `AddCors`/`UseCors` setup.
4. **ASP.NET Core routing** — matches the incoming URL + verb to `AthletesController.DeleteAthlete(int id)`, and calls it automatically (you never call this yourself).
5. **Controller** — calls `_context.Athletes.FindAsync(id)`, checks for null, then `_context.Athletes.Remove(athlete)`.
6. **EF Core** — `SaveChangesAsync()` translates the pending change into a real `DELETE FROM Athletes WHERE Id = @id` SQL command.
7. **SQL Server** — executes it, the row is gone.
8. **Response** — controller returns `NoContent()` (204), sent back over HTTP.
9. **RTK Query** — sees the mutation succeeded and that `deleteAthlete` invalidates the `Athlete` tag, which `getAthletes` provides — automatically re-fetches the list.
10. **React** — re-renders with the updated (shorter) list. No manual refresh, no manual state update anywhere in your code.

This same chain (React → RTK Query → CORS → ASP.NET Core → EF Core → SQL Server → back up) is the backbone of the entire app, for every CRUD operation.

---

## 9. Project Structure Reference

**Backend — `BoxingApp.Api`**
```
BoxingApp.Api/
├── Controllers/
│   └── AthletesController.cs      (GET, GET/{id}, POST, PUT, DELETE)
├── Models/
│   ├── Athlete.cs                 (Id, Name, WeightClass, Wins, Losses)
│   ├── TrainingPlan.cs            (Id, AthleteId, PlanName, navigation to Athlete)
│   └── CreateAthleteDto.cs        (Name, WeightClass, Wins, Losses — no Id, with validation attributes)
├── Data/
│   └── BoxingAppContext.cs        (DbSet<Athlete>, DbSet<TrainingPlan>)
├── Repositories/
│   └── AthleteRepository.cs       (first step toward Clean Architecture — in progress)
├── Migrations/                    (EF Core generated)
└── Program.cs                     (DbContext registration, CORS, Swagger)
```

**Frontend — `boxing-frontend`**
```
src/
├── App.jsx                        (layout: local Redux roster + live API section)
├── App.css                        (dark/red boxing theme, card grid)
├── AthleteCard.jsx                (Redux-connected card: winRecorded/winRemoved)
├── AthleteListFromApi.jsx         (RTK Query-connected: list, inline edit, delete)
├── CreateAthleteForm.jsx          (controlled form -> createAthlete mutation)
├── athletesSlice.js               (local Redux slice — fake/demo data)
├── apiSlice.js                    (RTK Query — real backend connection)
├── store.js                       (combines athletesSlice + apiSlice)
└── main.jsx                       (wraps App in <Provider store={store}>)
```

**Local dev setup:**
- SQL Server running in Docker (`sql-local` container), database `BoxingApp`
- Backend runs on `http://localhost:5213` (`dotnet run` in `BoxingApp.Api`)
- Frontend runs on `http://localhost:5173` (`npm run dev` in `boxing-frontend`)
- Both must run simultaneously for the full app to work

---

## 10. What's Left — Production-Readiness Roadmap

Decided order: **Clean Architecture → JWT Auth → Testing → Azure Deployment**

### Clean Architecture (in progress)
**The problem:** controllers currently talk directly to EF Core — database logic is scattered across every controller instead of centralized.
**The fix, one step at a time:**
1. ✅ Extract database calls into a `AthleteRepository` class (done — plain class, no interface yet)
2. Next: introduce an `IAthleteRepository` interface, so the controller depends on the *contract*, not the concrete class (ties back to the abstraction/DI concepts from Lesson 2)
3. Eventually: split into separate projects — Domain (entities, no dependencies), Application (interfaces/business logic), Infrastructure (EF Core implementation), Persistence (DbContext/migrations), API (controllers) — with dependencies only pointing inward toward Domain

### JWT Auth (not started)
- Login/register endpoints
- Protect POST/PUT/DELETE so only authenticated users can modify data
- Optional: role-based access (e.g. only "coaches" can edit athletes)

### Testing (not started)
- xUnit for unit tests (business logic in isolation)
- Integration tests hitting a real or in-memory database
- At least one test per CRUD operation

### Azure Deployment (paused mid-setup)
- Azure SQL Database (server + firewall rules configured for client IP + "allow Azure services")
- Azure App Service for the API
- Azure Static Web Apps (or similar) for the React frontend
- Update CORS to allow the deployed frontend's real URL
- Swap local connection string for the Azure SQL connection string
- Re-run EF Core migrations against Azure SQL

### Final polish
- Real `README.md` with architecture overview and setup instructions
- Clean GitHub commit history
- Remove any leftover sample/test code

---

## Quick Debugging Reference (patterns that came up repeatedly)

- **`CS0246` (type/namespace not found)** → almost always a missing `using` statement.
- **"Invalid hook call" / "Cannot read properties of null (reading 'useMemo')"** → duplicate or missing React/react-redux packages, not a code bug. Run `npm ls react react-redux` to check; clean reinstall (`node_modules` + `package-lock.json` deleted, then `npm install`) usually fixes it.
- **"X is not iterable"** on a mutation hook → forgot to actually call the hook: `useDeleteAthleteMutation()` not `useDeleteAthleteMutation`.
- **`IDENTITY_INSERT` error on POST** → the request body included an `Id` field; use a DTO that excludes it.
- **CORS errors in the browser console** → backend needs `AddCors`/`UseCors` configured for the frontend's exact origin.
