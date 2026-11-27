// components/ProjectForm.jsx
import { useState } from "react";
import { maakProject, wijzigProject } from "../api/projecten";
import { foutTekst } from "../api/client";

function ProjectForm({ project, onOpgeslagen, onAnnuleer }) {
  // project is {} bij nieuw, of een bestaand project bij wijzigen
  const [naam, setNaam] = useState(project.naam || "");
  const [categorie, setCategorie] = useState(project.categorie || "software");
  const [beschrijving, setBeschrijving] = useState(project.beschrijving || "");
  const [fouten, setFouten] = useState({});
  const [apiFout, setApiFout] = useState("");

  function controleer() {
    const nieuw = {};
    if (naam.trim() === "") nieuw.naam = "Vul een naam in.";
    if (beschrijving.trim().length < 10) nieuw.beschrijving = "De beschrijving moet minimaal 10 tekens hebben.";
    setFouten(nieuw);
    return Object.keys(nieuw).length === 0;
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setApiFout("");
    if (!controleer()) return;

    const data = { naam, categorie, beschrijving };

    try {
      if (project.id) {
        await wijzigProject(project.id, { id: project.id, ...data });
        onOpgeslagen("Project is gewijzigd.");
      } else {
        await maakProject(data);
        onOpgeslagen("Project is toegevoegd.");
      }
    } catch (err) {
      setApiFout(foutTekst(err.status));
    }
  }

  return (
    <form className="beheer-form" onSubmit={handleSubmit} noValidate>
      <h2>{project.id ? "Project wijzigen" : "Nieuw project"}</h2>

      <div className="form-group">
        <label htmlFor="project-naam">Naam</label>
        <input id="project-naam" value={naam} onChange={(e) => setNaam(e.target.value)}
               aria-invalid={fouten.naam ? "true" : undefined} aria-describedby="project-naam-fout" />
        <p className="error-message" id="project-naam-fout">{fouten.naam}</p>
      </div>

      <div className="form-group">
        <label htmlFor="project-categorie">Categorie</label>
        <select id="project-categorie" value={categorie} onChange={(e) => setCategorie(e.target.value)}>
          <option value="software">Software</option>
          <option value="web">Webontwikkeling</option>
          <option value="ai">AI</option>
        </select>
      </div>

      <div className="form-group">
        <label htmlFor="project-beschrijving">Beschrijving</label>
        <textarea id="project-beschrijving" rows="4" value={beschrijving}
                  onChange={(e) => setBeschrijving(e.target.value)}
                  aria-invalid={fouten.beschrijving ? "true" : undefined}
                  aria-describedby="project-beschrijving-fout" />
        <p className="error-message" id="project-beschrijving-fout">{fouten.beschrijving}</p>
      </div>

      <button className="button" type="submit">Opslaan</button>{" "}
      <button className="button knop-licht" type="button" onClick={onAnnuleer}>Annuleren</button>

      {apiFout && <p className="error-message" role="alert">{apiFout}</p>}
    </form>
  );
}

export default ProjectForm;