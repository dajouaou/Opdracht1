// components/Layout.jsx
import { Link, NavLink, Outlet, useNavigate } from "react-router-dom";
import { useAuth } from "../AuthContext";

function Layout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  async function handleLogout() {
    await logout();
    navigate("/");
  }

  return (
    <>
      <a className="skip-link" href="#main-content">Ga naar inhoud</a>

      <header className="site-header">
        <div className="container nav-container">
          <Link className="logo" to="/">Doa Ajouaou</Link>

          <nav aria-label="Hoofdnavigatie">
            <ul className="nav-list">
              <li><NavLink to="/" end>Whoami</NavLink></li>
              <li><NavLink to="/projecten">Projecten</NavLink></li>
              <li><NavLink to="/blog">Blog</NavLink></li>
              <li><NavLink to="/contact">Contact</NavLink></li>
              {user ? (
                <li>
                  <button className="nav-knop" onClick={handleLogout}>
                    Uitloggen ({user.email})
                  </button>
                </li>
              ) : (
                <>
                  <li><NavLink to="/login">Inloggen</NavLink></li>
                  <li><NavLink to="/registreren">Registreren</NavLink></li>
                </>
              )}
            </ul>
          </nav>
        </div>
      </header>

      <main id="main-content">
        <Outlet />
      </main>

      <footer className="site-footer">
        <div className="container">
          <p>&copy; 2026 Doa Ajouaou</p>
        </div>
      </footer>
    </>
  );
}

export default Layout;