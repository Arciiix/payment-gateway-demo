import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Form,
  FormControl,
  FormDescription,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { userQuery } from "@/queries/auth/user";
import { LoginData, loginSchema } from "@/schemas/auth/loginSchema";
import login from "@/services/auth/login";
import { User } from "@/types/auth/user";
import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Link, useNavigate } from "@tanstack/react-router";
import { HTTPError } from "ky";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

export function LoginForm() {
  const form = useForm<LoginData>({
    resolver: zodResolver(loginSchema),
  });

  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const { mutate, isPending } = useMutation({
    mutationFn: login,
    onError: (error) => {
      console.error(error);

      if (error instanceof HTTPError) {
        if (error.response.status === 401) {
          toast.error("Invalid password");
          return;
        } else if (error.response.status === 404) {
          toast.error("Invalid email");
          return;
        }
      }

      toast.error("An error occurred while trying to login.");
    },
    onSuccess: (data: User) => {
      toast.success("Logged in!");

      queryClient.setQueryData(userQuery.queryKey, data);

      navigate({ to: "/" });
    },
  });

  const onSubmit = (data: LoginData) => {
    mutate(data);
  };

  return (
    <Card className="mx-auto max-w-sm">
      <CardHeader>
        <CardTitle className="text-2xl">Login</CardTitle>
        <CardDescription>
          Please enter your email and password to login.
        </CardDescription>
      </CardHeader>
      <CardContent>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="grid gap-4">
            <div className="grid gap-4">
              <FormItem>
                <FormLabel htmlFor="email">Email</FormLabel>
                <FormControl>
                  <Input
                    id="email"
                    type="email"
                    placeholder="m@example.com"
                    required
                    {...form.register("email")}
                  />
                </FormControl>
                <FormDescription />
                <FormMessage>
                  {form.formState.errors.email?.message}
                </FormMessage>
              </FormItem>
              <FormItem>
                <FormLabel htmlFor="password">Password</FormLabel>
                <FormControl>
                  <Input
                    id="password"
                    type="password"
                    required
                    {...form.register("password")}
                  />
                </FormControl>
                <FormDescription />
                <FormMessage>
                  {form.formState.errors.password?.message}
                </FormMessage>
              </FormItem>
              <Button type="submit" className="w-full" disabled={isPending}>
                Login
              </Button>
            </div>
            <div className="mt-4 text-center text-sm">
              Don&apos;t have an account?{" "}
              <Link to="/auth/register" className="underline">
                Sign up
              </Link>
            </div>
          </form>
        </Form>
      </CardContent>
    </Card>
  );
}
