namespace PortfolioApi_Test;

public static class TestData
{
    public static object Project(string titel = "Testproject") => new
    {
        titel,
        beschrijving = "Een beschrijving",
        categorie = "Web",
        gitHubUrl = "https://github.com/voorbeeld/repo",
        datum = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
    };

    public static object BlogPost(string titel = "Testpost", DateTime? datum = null) => new
    {
        titel,
        inhoud = "Inhoud van de blogpost",
        publicatiedatum = datum ?? new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
    };
}
