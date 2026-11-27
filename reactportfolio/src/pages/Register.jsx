// pages/Register.jsx
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { registreer } from "../api/auth";
import PageHeader from "../components/PageHeader";

function Register() {
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [fout, setFout] = useState("");
  const [succes, setSucces] = useState("");

  async function handleSubmit(event) {
    event.preventDefault();
    setFout("");
    setSucces("");

    if (email.trim() === "" || password.length < 6) {
      setFout("Vul een e-mailadres in en een wachtwoord van minimaal 6 tekens.");
      return;
    }

    try {
      await registreer(email.trim(), password);
      setSucces("Account aangemaakt als Gast. Je kunt nu inloggen.");
      setTimeout(() => navigate("/login"), 1500);
    } catch {
      setFout("Registreren is mislukt. Het e-mailadres bestaat misschien al of het wachtwoord voldoet niet aan de regels.");
    }
  }

  return (
    <>
      <PageHeader eyebrow="Account" titel="Registreren" tekst="Maak een account aan. Je krijgt automatisch de rol Gast." />
      <section className="contact-section">
        <div className="container">
          <form onSubmit={handleSubmit} noValidate>
            <div className="form-group">
              <label htmlFor="reg-email">E-mailadres</label>
              <input id="reg-email" type="email" autoComplete="username" value={email} onChange={(e) => setEmail(e.target.value)} />
            </div>
            <div className="form-group">
              <label htmlFor="reg-password">Wachtwoord</label>
              <input id="reg-password" type="password" autoComplete="new-password" value={password} onChange={(e) => setPassword(e.target.value)} />
            </div>
            <button className="button" type="submit">Registreren</button>
            {fout && <p className="error-message" role="alert">{fout}</p>}
            {succes && <p role="status">{succes}</p>}
          </form>
        </div>
      </section>
    </>
  );
}

export default Register;