import { api } from "@/lib/fetcher";
import { RegisterData } from "@/schemas/auth/registerSchema";
import { User } from "@/types/auth/user";

export default async function register(data: RegisterData): Promise<User> {
  return api.post<User>("/api/auth/register", { json: data }).json();
}
