// projecten.js
import { request } from "./client";

export const getProjecten = () => request("/api/projects");
export const getProject = (id) => request(`/api/projects/${id}`);
export const maakProject = (project) =>
  request("/api/projects", { method: "POST", body: JSON.stringify(project) });
export const wijzigProject = (id, project) =>
  request(`/api/projects/${id}`, { method: "PUT", body: JSON.stringify(project) });
export const verwijderProject = (id) =>
  request(`/api/projects/${id}`, { method: "DELETE" });