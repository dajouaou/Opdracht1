// auth.js
import { request } from "./client";

export const registreer = (email, password) =>
  request("/api/auth/register", { method: "POST", body: JSON.stringify({ email, password }) });
export const inloggen = (email, password) =>
  request("/api/auth/login", { method: "POST", body: JSON.stringify({ email, password }) });
export const uitloggen = () => request("/api/auth/logout", { method: "POST" });
export const getMe = () => request("/api/auth/me");