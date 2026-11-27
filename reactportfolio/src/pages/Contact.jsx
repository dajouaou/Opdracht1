// pages/Contact.jsx
import { useState } from "react";
import PageHeader from "../components/PageHeader";

function Contact() {
  const [naam, setNaam] = useState("");
  const [email, setEmail] = useState("");
  const [bericht, setBericht] = useState("");
  const [fouten, setFouten] = useState({});
  const [status, setStatus] = useState("");

  function isGeldigEmail(waarde) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(waarde);
  }

  function handleSubmit(event) {
    event.preventDefault();

    const nieuw = {};
    if (naam.trim() === "") nieuw.naam = "Vul je naam in.";
    if (!isGeldigEmail(email.trim())) nieuw.email = "Vul een geldig e-mailadres in.";
    if (bericht.trim().length < 10) nieuw.bericht = "Je bericht moet minimaal 10 tekens bevatten.";
    setFouten(nieuw);

    if (Object.keys(nieuw).length > 0) {
      setStatus("Controleer de foutmeldingen bij de velden.");
      return;
    }

    setStatus("Bedankt! Je bericht is geldig ingevuld.");
    setNaam("");
    setEmail("");
    setBericht("");
  }

  return (
    <>
      <PageHeader eyebrow="Neem contact op" titel="Contact" tekst="Heb je een vraag? Vul dan het contactformulier in." />

      <section className="contact-section">
        <div className="container">
          <form onSubmit={handleSubmit} noValidate>
            <div className="form-group">
              <label htmlFor="name">Naam</label>
              <input type="text" id="name" value={naam} onChange={(e) => setNaam(e.target.value)}
                     aria-describedby="name-error" aria-invalid={fouten.naam ? "true" : undefined} />
              <p className="error-message" id="name-error">{fouten.naam}</p>
            </div>

            <div className="form-group">
              <label htmlFor="email">E-mailadres</label>
              <input type="email" id="email" value={email} onChange={(e) => setEmail(e.target.value)}
                     aria-describedby="email-error" aria-invalid={fouten.email ? "true" : undefined} />
              <p className="error-message" id="email-error">{fouten.email}</p>
            </div>

            <div className="form-group">
              <label htmlFor="message">Bericht</label>
              <textarea id="message" rows="5" value={bericht} onChange={(e) => setBericht(e.target.value)}
                        aria-describedby="message-error" aria-invalid={fouten.bericht ? "true" : undefined} />
              <p className="error-message" id="message-error">{fouten.bericht}</p>
            </div>

            <button className="button" type="submit">Verstuur bericht</button>
            <p id="form-status" role="status" aria-live="polite">{status}</p>
          </form>
        </div>
      </section>
    </>
  );
}

export default Contact;