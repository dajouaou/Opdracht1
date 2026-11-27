// pages/Login.jsx
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../AuthContext";
import PageHeader from "../components/PageHeader";

function Login() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [fout, setFout] = useState("");

  async function handleSubmit(event) {
    event.preventDefault();
    setFout("");

    if (email.trim() === "" || password === "") {
      setFout("Vul je e-mailadres en wachtwoord in.");
      return;
    }

    try {
      await login(email.trim(), password);
      navigate("/projecten");
    } catch {
      setFout("Inloggen is mislukt. Controleer je e-mailadres en wachtwoord.");
    }
  }

  return (
    <>
      <PageHeader eyebrow="Account" titel="Inloggen" tekst="Log in om als Editor of Admin je portfolio te beheren." />
      <section className="contact-section">
        <div className="container">
          <form onSubmit={handleSubmit} noValidate>
            <div className="form-group">
              <label htmlFor="login-email">E-mailadres</label>
              <input id="login-email" type="email" autoComplete="username" value={email} onChange={(e) => setEmail(e.target.value)} />
            </div>
            <div className="form-group">
              <label htmlFor="login-password">Wachtwoord</label>
              <input id="login-password" type="password" autoComplete="current-password" value={password} onChange={(e) => setPassword(e.target.value)} />
            </div>
            <button className="button" type="submit">Inloggen</button>
            {fout && <p className="error-message" role="alert">{fout}</p>}
          </form>
        </div>
      </section>
    </>
  );
}

export default Login;