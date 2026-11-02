
const projecten = [
    {
        naam: "Java applicatie voor Dirk",
        categorie: "software",
        beschrijving: "Een Java applicatie voor het beheren van de voorraad van producten."
    },
    {
        naam: "Teamflow",
        categorie: "software",
        beschrijving: "Een chatapplicatie voor Scrum-teams waarbij berichten gekoppeld kunnen worden aan user stories, taken en epics."
    },
    {
        naam: "Digitale veilingklok",
        categorie: "web",
        beschrijving: "Een digitale veilingklok voor bloemen en planten bij Royal FloraHolland."
    },
    {
        naam: "AI Agent voor studenten",
        categorie: "ai",
        beschrijving: "Een AI-agent die studenten helpt met vragen over hun lesstof."
    }
];

// selecteer de onderdelen die de gebruiker kan aanpassen
const projectsList = document.querySelector("#projects-list");
const categoryFilter = document.querySelector("#category-filter");
const projectSearch = document.querySelector("#project-search");
const projectStatus = document.querySelector("#project-status");

// maak een projectkaart aan en voeg deze toe aan de pagina
function createProjectCard(project) {
    const article = document.createElement("article");
    article.classList.add("project-card");

    const title = document.createElement("h2");
    title.textContent = project.naam;

    const category = document.createElement("p");
    category.textContent = `Categorie: ${project.categorie}`;

    const description = document.createElement("p");
    description.textContent = project.beschrijving;

    article.append(title, category, description);

    return article;
}

// toon de projecten die bij het filter en de zoekterm passen
function renderProjects() {
    const selectedCategory = categoryFilter.value;
    const searchTerm = projectSearch.value.toLowerCase().trim();

    const filteredProjects = projecten.filter(project => {
        const matchesCategory =
            selectedCategory === "alle" ||
            project.categorie === selectedCategory;

        const matchesSearch =
            project.naam.toLowerCase().includes(searchTerm) ||
            project.beschrijving.toLowerCase().includes(searchTerm);

        return matchesCategory && matchesSearch;
    });

    // verwijder de oude kaarten voordat de nieuwe worden getoond
    projectsList.replaceChildren();

    filteredProjects.forEach(project => {
        const card = createProjectCard(project);
        projectsList.appendChild(card);
    });

    projectStatus.textContent =
        `${filteredProjects.length} project(en) gevonden.`;
}

// reageer op veranderingen in het filter en de zoekbalk
categoryFilter.addEventListener("change", renderProjects);
projectSearch.addEventListener("input", renderProjects);

// toon de projecten zodra de pagina wordt geopend
renderProjects();
