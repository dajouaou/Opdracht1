import { createContext, useContext, useEffect, useState } from "react";
import { getMe, inloggen, uitloggen } from "./api/auth";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [laden, setLaden] = useState(true);

  // na een verversing aan de API vragen of we nog ingelogd zijn
  useEffect(() => {
    getMe()
      .then(setUser)
      .catch(() => setUser(null))
      .finally(() => setLaden(false));
  }, []);

  async function login(email, password) {
    await inloggen(email, password);
    setUser(await getMe());
  }

  async function logout() {
    await uitloggen();
    setUser(null);
  }

  const isBeheerder =
    user !== null && user.roles.some((r) => r === "Editor" || r === "Admin");

  return (
    <AuthContext.Provider value={{ user, laden, login, logout, isBeheerder }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}