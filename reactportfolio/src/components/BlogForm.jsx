// components/BlogForm.jsx
import { useState } from "react";
import { maakBlog, wijzigBlog } from "../api/blog";
import { foutTekst } from "../api/client";

function BlogForm({ blog, onOpgeslagen, onAnnuleer }) {
  const [titel, setTitel] = useState(blog.titel || "");
  const [inhoud, setInhoud] = useState(blog.inhoud || "");
  const [fouten, setFouten] = useState({});
  const [apiFout, setApiFout] = useState("");

  function controleer() {
    const nieuw = {};
    if (titel.trim() === "") nieuw.titel = "Vul een titel in.";
    if (inhoud.trim().length < 10) nieuw.inhoud = "De tekst moet minimaal 10 tekens hebben.";
    setFouten(nieuw);
    return Object.keys(nieuw).length === 0;
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setApiFout("");
    if (!controleer()) return;

    const data = { titel, inhoud };

    try {
      if (blog.id) {
        await wijzigBlog(blog.id, { id: blog.id, ...data });
        onOpgeslagen("Blogpost is gewijzigd.");
      } else {
        await maakBlog(data);
        onOpgeslagen("Blogpost is toegevoegd.");
      }
    } catch (err) {
      setApiFout(foutTekst(err.status));
    }
  }

  return (
    <form className="beheer-form" onSubmit={handleSubmit} noValidate>
      <h2>{blog.id ? "Blogpost wijzigen" : "Nieuwe blogpost"}</h2>

      <div className="form-group">
        <label htmlFor="blog-titel">Titel</label>
        <input id="blog-titel" value={titel} onChange={(e) => setTitel(e.target.value)}
               aria-invalid={fouten.titel ? "true" : undefined} aria-describedby="blog-titel-fout" />
        <p className="error-message" id="blog-titel-fout">{fouten.titel}</p>
      </div>

      <div className="form-group">
        <label htmlFor="blog-inhoud">Tekst</label>
        <textarea id="blog-inhoud" rows="5" value={inhoud} onChange={(e) => setInhoud(e.target.value)}
                  aria-invalid={fouten.inhoud ? "true" : undefined} aria-describedby="blog-inhoud-fout" />
        <p className="error-message" id="blog-inhoud-fout">{fouten.inhoud}</p>
      </div>

      <button className="button" type="submit">Opslaan</button>{" "}
      <button className="button knop-licht" type="button" onClick={onAnnuleer}>Annuleren</button>

      {apiFout && <p className="error-message" role="alert">{apiFout}</p>}
    </form>
  );
}

export default BlogForm;