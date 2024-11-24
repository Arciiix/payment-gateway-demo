import { api } from "@/lib/fetcher";
import { User } from "@/types/auth/user";

export default async function fetchUser(): Promise<User | null> {
  try {
    const response = await api.get<User>("/api/auth/user");
    return response.json();
  } catch (error) {
    console.log("fetchUser error");
    console.error(error);
    // if (error instanceof Response) {
    //   if (error.status === 401) {
    //     return null;
    //   }
    // }
    // throw error;
    return null;
  }
}
