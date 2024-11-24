import { api } from "@/lib/fetcher";
import { LoginData } from "@/schemas/auth/loginSchema";
import { User } from "@/types/auth/user";

export default async function login(data: LoginData): Promise<User> {
  return api.post<User>("/api/auth/login", { json: data }).json();
}
