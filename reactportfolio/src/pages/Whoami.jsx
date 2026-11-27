// pages/Whoami.jsx
import { Link } from "react-router-dom";
import PageHeader from "../components/PageHeader";

function Whoami() {
  return (
    <>
      <PageHeader
        eyebrow="Even voorstellen"
        titel="Whoami"
        tekst="Hoi, ik ben Doa. Ik ben 21 jaar en student. Op deze website laat ik zien wie ik ben en aan welke projecten ik heb gewerkt."
      />

      <section className="about-section">
        <div className="container about-content">
          <div className="about-card">
            <h2>Over mij</h2>
            <p>
              Ik vind het leuk om bezig te zijn met technologie en software.
              Tijdens mijn opleiding leer ik steeds meer over programmeren,
              databases, websites en AI.
            </p>
            <p>
              Ik vind het vooral leuk om iets te maken en daarna te kijken
              hoe ik het beter en makkelijker kan maken.
            </p>
          </div>

          <div className="about-card">
            <h2>Wat ik leer</h2>
            <ul className="skills-list">
              <li>C# programmeren</li>
              <li>Websites maken met HTML, CSS en React</li>
              <li>Werken met databases</li>
              <li>Werken met AI</li>
              <li>Projectmatig samenwerken</li>
            </ul>
          </div>
        </div>
      </section>

      <section className="intro-section">
        <div className="container">
          <h2>Mijn portfolio</h2>
          <p>Bekijk mijn projecten om te zien wat ik tijdens mijn opleiding heb gemaakt.</p>
          <Link className="button" to="/projecten">Bekijk mijn projecten</Link>
        </div>
      </section>
    </>
  );
}

export default Whoami;