// pages/Blog.jsx
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getBlogs, verwijderBlog } from "../api/blog";
import { foutTekst } from "../api/client";
import { useAuth } from "../AuthContext";
import PageHeader from "../components/PageHeader";
import BlogForm from "../components/BlogForm";

function Blog() {
  const { isBeheerder } = useAuth();
  const [blogs, setBlogs] = useState([]);
  const [status, setStatus] = useState("laden");
  const [formulier, setFormulier] = useState(null);
  const [melding, setMelding] = useState("");

  async function laadBlogs() {
    try {
      setBlogs(await getBlogs());
      setStatus("klaar");
    } catch {
      setStatus("fout");
    }
  }

  useEffect(() => {
    laadBlogs();
  }, []);

  async function verwijder(blog) {
    if (!window.confirm(`Weet je zeker dat je "${blog.titel}" wilt verwijderen?`)) return;
    try {
      await verwijderBlog(blog.id);
      setMelding("Blogpost is verwijderd.");
      laadBlogs();
    } catch (err) {
      setMelding(foutTekst(err.status));
    }
  }

  function opgeslagen(tekst) {
    setFormulier(null);
    setMelding(tekst);
    laadBlogs();
  }

  return (
    <>
      <PageHeader
        eyebrow="Mijn ervaringen"
        titel="Blog"
        tekst="Hier schrijf ik kort over dingen die ik leer en projecten waar ik mee bezig ben."
      />

      <section className="blog-section">
        <div className="container">
          {isBeheerder && formulier === null && (
            <button className="button nieuw-knop" onClick={() => setFormulier({})}>Nieuwe blogpost</button>
          )}

          {formulier !== null && (
            <BlogForm blog={formulier} onOpgeslagen={opgeslagen} onAnnuleer={() => setFormulier(null)} />
          )}

          <p role="status" aria-live="polite">{melding}</p>

          {status === "laden" && <p>Blogposts worden geladen...</p>}
          {status === "fout" && <p className="error-message" role="alert">De blogposts konden niet worden opgehaald. Draait de API?</p>}
          {status === "klaar" && blogs.length === 0 && <p>Er zijn nog geen blogposts.</p>}

          {blogs.map((blog) => (
            <article className="blog-card" key={blog.id}>
              <div className="blog-date">Blog {blog.id}</div>
              <h2><Link to={`/blog/${blog.id}`}>{blog.titel}</Link></h2>
              <p>{blog.inhoud}</p>

              {isBeheerder && (
                <div className="knoppen">
                  <button className="button" onClick={() => setFormulier(blog)}>Wijzig</button>
                  <button className="button knop-gevaar" onClick={() => verwijder(blog)}>Verwijder</button>
                </div>
              )}
            </article>
          ))}
        </div>
      </section>
    </>
  );
}

export default Blog;