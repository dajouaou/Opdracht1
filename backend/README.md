# Portfolio Backend

## Over mijn project

Voor opdracht 4 heb ik mijn portfolio-backend uitgebreid met beveiliging en tests. Met deze backend kunnen projecten en blogposts worden aangemaakt, opgevraagd, aangepast en verwijderd. Lezen mag iedereen; schrijven alleen een ingelogde gebruiker met de rol Editor of Admin.

Ik heb hiervoor ASP.NET Core Web API, Entity Framework Core, SQL Server en ASP.NET Core Identity gebruikt. De tests staan in een apart xUnit-project in dezelfde solution.

## Gebruikte technieken

- .NET 10
- ASP.NET Core Web API
- C#
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- xUnit
- Swagger UI

## Projectstructuur

De backend is opgebouwd met een meerlagenarchitectuur. Hierdoor heeft elk onderdeel een eigen verantwoordelijkheid.

| Map | Beschrijving |
| Controllers | Bevat de controllers die HTTP-requests afhandelen |
| Services | Bevat de applicatielogica en CRUD-bewerkingen |
| DTOs | Bevat de DTO's voor het ontvangen en terugsturen van gegevens |
| Models | Bevat de entiteiten die in de database worden opgeslagen |
| Data | Bevat de DbContext voor de communicatie met de database |
| Identity | Bevat de rolseeder die de rollen Gast, Editor en Admin aanmaakt |
| Migrations | Bevat de migraties voor de databasestructuur |
| Tests | Bevat het xUnit-testproject met de HTTP-tests |

## Functionaliteiten

De API ondersteunt volledige CRUD-functionaliteit voor projecten en blogposts, plus registratie en login met Identity.

### Projecten

| Methode | Endpoint | Beschrijving | Toegang |
| GET | `/api/projects` | Alle projecten ophalen | Publiek |
| GET | `/api/projects/{id}` | Eén project ophalen | Publiek |
| POST | `/api/projects` | Een nieuw project aanmaken | Editor of Admin |
| PUT | `/api/projects/{id}` | Een project wijzigen | Editor of Admin |
| DELETE | `/api/projects/{id}` | Een project verwijderen | Editor of Admin |

### Blogposts

| Methode | Endpoint | Beschrijving | Toegang |
| GET | `/api/blogposts` | Alle blogposts ophalen | Publiek |
| GET | `/api/blogposts/{id}` | Eén blogpost ophalen | Publiek |
| POST | `/api/blogposts` | Een nieuwe blogpost aanmaken | Editor of Admin |
| PUT | `/api/blogposts/{id}` | Een blogpost wijzigen | Editor of Admin |
| DELETE | `/api/blogposts/{id}` | Een blogpost verwijderen | Editor of Admin |

### Statuscodes bij beveiligde routes

| Situatie | Resultaat |
| Zonder aanmelding een POST, PUT of DELETE | 401 Unauthorized |
| Ingelogd als Gast een POST, PUT of DELETE | 403 Forbidden |
| Ingelogd als Editor of Admin | 201 bij aanmaken, 204 bij wijzigen en verwijderen |
| Onbekend id bij GET, PUT of DELETE | 404 Not Found |

## Beveiliging met ASP.NET Core Identity

Voor opdracht 4 heb ik ASP.NET Core Identity met EF Core toegevoegd. Gebruikers worden opgeslagen in dezelfde SQL Server-database als mijn projecten en blogposts. Mijn `PortfolioDbContext` erft daarom van `IdentityDbContext<IdentityUser>`, zodat de Identity-tabellen automatisch worden meegenomen in de migraties.

### Rollen

De API kent drie rollen:

| Rol | Rechten |
| Gast | Standaardrol bij registratie. Mag alleen lezen (GET). |
| Editor | Mag projecten en blogposts aanmaken, wijzigen en verwijderen. |
| Admin | Heeft dezelfde schrijfrechten als Editor, omdat Admin expliciet in de autorisatieregel staat. |

Bij het opstarten worden de rollen automatisch aangemaakt via `RolSeeder.SeedRolesAsync`, dat gebruikmaakt van de `RoleManager`. Dit gebeurt alleen als een rol nog niet bestaat, dus het is veilig om meerdere keren te starten.

### Registratie en login

| Methode | Endpoint | Beschrijving |
| POST | `/api/auth/register` | Nieuwe gebruiker aanmaken. Krijgt automatisch de rol Gast. |
| POST | `/api/auth/login` | Inloggen. Geeft een authenticatiecookie terug. |
| POST | `/api/auth/logout` | Uitloggen (alleen voor ingelogde gebruikers). |
| GET | `/api/auth/me` | Toont het e-mailadres en de rollen van de ingelogde gebruiker. |

De registratie- en loginroutes heb ik zelf gebouwd met de `UserManager` en de `SignInManager`. Publieke registratie kent nooit de rol Editor of Admin toe; de rol Gast wordt server-side toegevoegd en is niet vanuit de request te beïnvloeden.

### De policy EditorOrAdmin

Alle schrijfroutes zijn beveiligd met één policy. Die staat in `Program.cs`:

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EditorOrAdmin", policy =>
        policy.RequireRole("Editor", "Admin"));
});