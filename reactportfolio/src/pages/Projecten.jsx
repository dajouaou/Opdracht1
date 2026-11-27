// pages/Projecten.jsx
import { useEffect, useState } from "react";
import { getProjecten, verwijderProject } from "../api/projecten";
import { foutTekst } from "../api/client";
import { useAuth } from "../AuthContext";
import PageHeader from "../components/PageHeader";
import ProjectCard from "../components/ProjectCard";
import ProjectForm from "../components/ProjectForm";
import Weer from "../components/Weer";

function Projecten() {
  const { isBeheerder } = useAuth();
  const [projecten, setProjecten] = useState([]);
  const [status, setStatus] = useState("laden"); // laden, klaar of fout
  const [categorie, setCategorie] = useState("alle");
  const [zoek, setZoek] = useState("");
  const [formulier, setFormulier] = useState(null); // null = verborgen
  const [melding, setMelding] = useState("");

  async function laadProjecten() {
    try {
      setProjecten(await getProjecten());
      setStatus("klaar");
    } catch {
      setStatus("fout");
    }
  }

  useEffect(() => {
    laadProjecten();
  }, []);

  async function verwijder(project) {
    if (!window.confirm(`Weet je zeker dat je "${project.naam}" wilt verwijderen?`)) return;
    try {
      await verwijderProject(project.id);
      setMelding("Project is verwijderd.");
      laadProjecten();
    } catch (err) {
      setMelding(foutTekst(err.status));
    }
  }

  function opgeslagen(tekst) {
    setFormulier(null);
    setMelding(tekst);
    laadProjecten();
  }

  const zoekterm = zoek.toLowerCase().trim();
  const gefilterd = projecten.filter((p) => {
    const pastCategorie = categorie === "alle" || p.categorie === categorie;
    const pastZoek =
      p.naam.toLowerCase().includes(zoekterm) ||
      p.beschrijving.toLowerCase().includes(zoekterm);
    return pastCategorie && pastZoek;
  });

  return (
    <>
      <PageHeader
        eyebrow="Mijn werk"
        titel="Projecten"
        tekst="Een overzicht van projecten waar ik tijdens mijn opleiding aan heb gewerkt."
      />

      <section className="projects-section">
        <div className="container">
          <div className="project-controls">
            <label htmlFor="category-filter">Filter op categorie</label>
            <select id="category-filter" value={categorie} onChange={(e) => setCategorie(e.target.value)}>
              <option value="alle">Alle projecten</option>
              <option value="software">Software</option>
              <option value="web">Webontwikkeling</option>
              <option value="ai">AI</option>
            </select>

            <label htmlFor="project-search">Zoek een project</label>
            <input type="search" id="project-search" placeholder="Typ een projectnaam..."
                   value={zoek} onChange={(e) => setZoek(e.target.value)} />
          </div>

          {isBeheerder && formulier === null && (
            <button className="button nieuw-knop" onClick={() => setFormulier({})}>Nieuw project</button>
          )}

          {formulier !== null && (
            <ProjectForm project={formulier} onOpgeslagen={opgeslagen} onAnnuleer={() => setFormulier(null)} />
          )}

          <p id="project-status" role="status" aria-live="polite">{melding}</p>

          {status === "laden" && <p>Projecten worden geladen...</p>}
          {status === "fout" && <p className="error-message" role="alert">De projecten konden niet worden opgehaald. Draait de API?</p>}
          {status === "klaar" && projecten.length === 0 && <p>Er zijn nog geen projecten.</p>}
          {status === "klaar" && projecten.length > 0 && gefilterd.length === 0 && <p>Geen projecten gevonden voor dit filter.</p>}

          <div className="projects-grid">
            {gefilterd.map((project) => (
              <ProjectCard key={project.id} project={project} isBeheerder={isBeheerder}
                           onWijzig={setFormulier} onVerwijder={verwijder} />
            ))}
          </div>
        </div>
      </section>

      <Weer />
    </>
  );
}

export default Projecten;