# Onderbouwing ontwerpkeuzes

## Studentgegevens

- Naam: Doa Ajouaou
- Leeftijd: 20 jaar
- Opdracht: WPFW Opdracht 1

## Gebruikersscenario's

### Scenario 1 – Recruiter bekijkt projecten

**Situatie:** Een recruiter opent de portfolio-site om snel te zien wat Doa heeft gemaakt.

**Doel:** De recruiter moet zonder zoeken bij de projecten kunnen komen en per project snel kunnen begrijpen wat het project inhoudt.

**Ondersteuning in het ontwerp:**
- De hoofdnavigatie bevat direct een link naar "Projecten".
- Projecten zijn als afzonderlijke kaarten opgebouwd.
- Elk project heeft een duidelijke titel, korte uitleg en kernpunten.

### Scenario 2 – Bezoeker leest over ontwikkeling

**Situatie:** Een bezoeker wil niet alleen projecten bekijken, maar ook zien wat Doa leert en waarop zij reflecteert.

**Doel:** De bezoeker moet gemakkelijk naar de blog kunnen gaan en berichten kunnen scannen.

**Ondersteuning in het ontwerp:**
- "Blog" staat in dezelfde hoofdnavigatie als Whoami en Projecten.
- Elk blogbericht heeft een duidelijke titel, datum en korte introductie.
- De drie berichten zijn visueel gelijk opgebouwd, waardoor de pagina snel scanbaar is.

## Drie belangrijke ontwerpkeuzes

### 1. Semantische HTML-structuur

Ik gebruik onder andere `header`, `nav`, `main`, `section`, `article` en `footer`. Hierdoor beschrijft de HTML de betekenis en structuur van de inhoud in plaats van alleen de visuele vorm.

MDN beschrijft deze elementen als structurele onderdelen van een document en legt uit dat semantische structuur de toegankelijkheid kan verbeteren doordat ondersteunende technologieën de structuur beter kunnen herkennen.

Bron: MDN – Structuring documents:
https://developer.mozilla.org/en-US/docs/Learn_web_development/Core/Structuring_content/Structuring_documents

### 2. Responsive layout

Ik gebruik CSS Grid en Flexbox en heb twee belangrijke breekpunten toegevoegd: rond 768 px en rond 480 px. Op kleinere schermen worden kolommen onder elkaar geplaatst en blijft de navigatie bruikbaar.

Dit ondersteunt beide gebruikersscenario's omdat een recruiter of bezoeker de site ook vanaf een tablet of telefoon moet kunnen gebruiken.

### 3. Duidelijke navigatie en focus

De drie hoofdonderdelen staan steeds in dezelfde navigatie: Whoami, Projecten en Blog. De huidige pagina krijgt een duidelijke actieve status. Daarnaast is een skip-link toegevoegd waarmee keyboard- en screenreadergebruikers direct naar de hoofdinhoud kunnen gaan.

De keuze voor `nav` sluit aan bij MDN, dat `nav` beschrijft als het element voor een belangrijk navigatieblok. De `main`-sectie geeft de hoofdinhoud van de pagina aan.

Bronnen:
- MDN – nav: https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/nav
- MDN – main: https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/main

## Toegankelijkheid

- De pagina's hebben een `lang="nl"`-attribuut.
- De viewport is ingesteld voor mobiele apparaten.
- Koppen hebben een logische hiërarchie.
- Links hebben duidelijke teksten.
- Er is een zichtbare keyboard-focus.
- Er is een skip-link.
- Er zijn geen decoratieve afbeeldingen nodig; daardoor zijn er in deze versie ook geen betekenisloze afbeeldingen zonder alt-tekst.
- De tekst- en achtergrondkleuren zijn gekozen met voldoende contrast als uitgangspunt. Dit moet vóór definitieve inlevering nog met een contrastchecker worden gecontroleerd.

## Bestandsstructuur

De website gebruikt losse HTML-bestanden en één centrale CSS-map:

```text
WPFW_Opdracht_1_Doa_Ajouaou/
├── index.html
├── projecten.html
├── blog.html
├── css/
│   └── style.css
├── README.md
├── request-response.md
├── onderbouwing.md
└── procesverantwoording.md
```

Hierdoor kunnen later eenvoudig nieuwe projecten of blogposts worden toegevoegd zonder de bestaande structuur opnieuw op te bouwen.
