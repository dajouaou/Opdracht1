# Portfolio Backend

## Over mijn project

Voor opdracht 3 heb ik een backend gemaakt voor mijn portfoliosite. Met deze backend kunnen projecten en blogposts worden aangemaakt, opgevraagd, aangepast en verwijderd.

Ik heb hiervoor ASP.NET Core Web API, Entity Framework Core en SQL Server gebruikt.

## Gebruikte technieken

- .NET 10
- ASP.NET Core Web API
- C#
- Entity Framework Core
- SQL Server
- Swagger UI

## Projectstructuur

De backend is opgebouwd met een meerlagenarchitectuur. Hierdoor heeft elk onderdeel een eigen verantwoordelijkheid.

| Map | Beschrijving |
| Controllers | Bevat de controllers die HTTP-requests afhandelen |
| Services | Bevat de applicatielogica en CRUD-bewerkingen |
| DTOs | Bevat de DTO's voor het ontvangen en terugsturen van gegevens |
| Models | Bevat de entiteiten die in de database worden opgeslagen |
| Data | Bevat de DbContext voor de communicatie met de database |
| Migrations | Bevat de migraties voor de databasestructuur |

## Functionaliteiten

De API ondersteunt volledige CRUD-functionaliteit voor projecten en blogposts.

### Projecten

| Methode | Endpoint | Beschrijving |
| GET | `/api/projects` | Alle projecten ophalen |
| GET | `/api/projects/{id}` | Eén project ophalen |
| POST | `/api/projects` | Een nieuw project aanmaken |
| PUT | `/api/projects/{id}` | Een project wijzigen |
| DELETE | `/api/projects/{id}` | Een project verwijderen |

### Blogposts

| Methode | Endpoint | Beschrijving |
| GET | `/api/blogposts` | Alle blogposts ophalen |
| GET | `/api/blogposts/{id}` | Eén blogpost ophalen |
| POST | `/api/blogposts` | Een nieuwe blogpost aanmaken |
| PUT | `/api/blogposts/{id}` | Een blogpost wijzigen |
| DELETE | `/api/blogposts/{id}` | Een blogpost verwijderen |

## Database instellen

De backend gebruikt SQL Server om projecten en blogposts op te slaan.

De connectionstring staat in `appsettings.json`. Pas deze aan als je een andere SQL Server gebruikt.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PortfolioDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

De database wordt aangemaakt met Entity Framework Core migrations.

Voer in de terminal de volgende opdracht uit vanuit de projectmap:

```bash
dotnet ef database update
```

Als het `dotnet ef`-commando niet beschikbaar is, installeer dan eerst de Entity Framework Core CLI-tool.

## Applicatie starten

1. Open de projectmap in Visual Studio Code.
2. Open een terminal in de map waarin het `.csproj`-bestand staat.
3. Controleer of .NET 10 is geïnstalleerd.
4. Voer de volgende opdracht uit:

```bash
dotnet restore
```

5. Maak de database aan of werk deze bij:

```bash
dotnet ef database update
```

6. Start de applicatie:

```bash
dotnet run
```

De applicatie is nu gestart.

## Swagger UI

Met Swagger UI kan ik de endpoints testen zonder een aparte frontend te gebruiken.

Open na het starten van de applicatie de Swagger-link die in de terminal wordt getoond.

Via Swagger kan ik projecten en blogposts aanmaken, opvragen, wijzigen en verwijderen.

## Repository

De code van dit project staat in mijn repository:

[link naar repo](https://github.com/dajouaou/Opdracht1)

## Auteur

Gemaakt door: [Doa Ajouaou] 

Opleiding: HBO-ICT

Opdracht: Opdracht 3 – Backend met ASP.NET Core Web API en EF Core
