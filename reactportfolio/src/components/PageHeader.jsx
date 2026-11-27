// components/PageHeader.jsx
function PageHeader({ eyebrow, titel, tekst }) {
  return (
    <section className="page-header">
      <div className="container">
        <p className="eyebrow">{eyebrow}</p>
        <h1>{titel}</h1>
        <p>{tekst}</p>
      </div>
    </section>
  );
}

export default PageHeader;