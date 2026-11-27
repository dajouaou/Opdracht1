// App.jsx
import { Routes, Route } from "react-router-dom";
import Layout from "./components/Layout";
import Whoami from "./pages/Whoami";
import Projecten from "./pages/Projecten";
import ProjectDetail from "./pages/ProjectDetail";
import Blog from "./pages/Blog";
import BlogDetail from "./pages/BlogDetail";
import Contact from "./pages/Contact";
import Login from "./pages/Login";
import Register from "./pages/Register";

function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route path="/" element={<Whoami />} />
        <Route path="/projecten" element={<Projecten />} />
        <Route path="/projecten/:id" element={<ProjectDetail />} />
        <Route path="/blog" element={<Blog />} />
        <Route path="/blog/:id" element={<BlogDetail />} />
        <Route path="/contact" element={<Contact />} />
        <Route path="/login" element={<Login />} />
        <Route path="/registreren" element={<Register />} />
        <Route path="*" element={<p className="container status-tekst">Pagina niet gevonden.</p>} />
      </Route>
    </Routes>
  );
}

export default App;