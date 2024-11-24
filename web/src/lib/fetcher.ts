import ky from "ky";

export const api = ky.extend({
  credentials: "include",
  headers: {
    "Content-Type": "application/json",
  },
});
