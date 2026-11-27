// pages/BlogDetail.jsx
import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { getBlog } from "../api/blog";
import PageHeader from "../components/PageHeader";

function BlogDetail() {
  const { id } = useParams();
  const [blog, setBlog] = useState(null);
  const [status, setStatus] = useState("laden");

  useEffect(() => {
    let actueel = true;
    setStatus("laden");

    getBlog(id)
      .then((data) => {
        if (actueel) {
          setBlog(data);
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
      <PageHeader eyebrow="Blog" titel={status === "klaar" ? blog.titel : "Blog"} tekst="" />
      <section className="blog-section">
        <div className="container">
          {status === "laden" && <p>Blogpost wordt geladen...</p>}
          {status === "onbekend" && <p className="error-message" role="alert">Deze blogpost bestaat niet.</p>}
          {status === "fout" && <p className="error-message" role="alert">De blogpost kon niet worden opgehaald.</p>}
          {status === "klaar" && (
            <article className="blog-card">
              <p>{blog.inhoud}</p>
            </article>
          )}
          <p><Link to="/blog">← Terug naar blog</Link></p>
        </div>
      </section>
    </>
  );
}

export default BlogDetail;