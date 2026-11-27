// components/ProjectCard.jsx
import { Link } from "react-router-dom";

function ProjectCard({ project, isBeheerder, onWijzig, onVerwijder }) {
  return (
    <article className="project-card">
      <h2><Link to={`/projecten/${project.id}`}>{project.naam}</Link></h2>
      <p>Categorie: {project.categorie}</p>
      <p>{project.beschrijving}</p>

      {isBeheerder && (
        <div className="knoppen">
          <button className="button" onClick={() => onWijzig(project)}>Wijzig</button>
          <button className="button knop-gevaar" onClick={() => onVerwijder(project)}>Verwijder</button>
        </div>
      )}
    </article>
  );
}

export default ProjectCard;