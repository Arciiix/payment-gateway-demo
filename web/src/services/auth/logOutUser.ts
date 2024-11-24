import { api } from "@/lib/fetcher";

export default async function logOutUser(): Promise<void> {
  try {
    await api.delete("/api/auth/logout");
  } catch (error) {
    console.log("logOutUser error");
    console.error(error);
  }
}
