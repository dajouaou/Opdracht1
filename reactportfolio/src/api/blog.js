// blog.js
import { request } from "./client";

export const getBlogs = () => request("/api/blogposts");
export const getBlog = (id) => request(`/api/blogposts/${id}`);
export const maakBlog = (blog) =>
  request("/api/blogposts", { method: "POST", body: JSON.stringify(blog) });
export const wijzigBlog = (id, blog) =>
  request(`/api/blogposts/${id}`, { method: "PUT", body: JSON.stringify(blog) });
export const verwijderBlog = (id) =>
  request(`/api/blogposts/${id}`, { method: "DELETE" });