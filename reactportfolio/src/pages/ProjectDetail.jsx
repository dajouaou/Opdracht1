// pages/ProjectDetail.jsx
import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { getProject } from "../api/projecten";
import PageHeader from "../components/PageHeader";

function ProjectDetail() {
  const { id } = useParams();
  const [project, setProject] = useState(null);
  const [status, setStatus] = useState("laden"); // laden, klaar, onbekend of fout

  useEffect(() => {
    // actueel voorkomt dat een traag antwoord van een eerder verzoek het scherm overschrijft
    let actueel = true;
    setStatus("laden");

    getProject(id)
      .then((data) => {
        if (actueel) {
          setProject(data);
          setStatus("klaar");
        }
      })
      .catch((err) => {
        if (actueel) setStatus(err.status === 404 ? "onbekend" : "fout");
      });

    return () => {
      actueel = false;
    };
  }, [id]);

  return (
    <>
      <PageHeader eyebrow="Project" titel={status === "klaar" ? project.naam : "Project"} tekst="" />
      <section className="projects-section">
        <div className="container">
          {status === "laden" && <p>Project wordt geladen...</p>}
          {status === "onbekend" && <p className="error-message" role="alert">Dit project bestaat niet.</p>}
          {status === "fout" && <p className="error-message" role="alert">Het project kon niet worden opgehaald.</p>}
          {status === "klaar" && (
            <article className="project-card">
              <p>Categorie: {project.categorie}</p>
              <p>{project.beschrijving}</p>
            </article>
          )}
          <p style={{ marginTop: 20 }}><Link to="/projecten">← Terug naar projecten</Link></p>
        </div>
      </section>
    </>
  );
}

export default ProjectDetail;