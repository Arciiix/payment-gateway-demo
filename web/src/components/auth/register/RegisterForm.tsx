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
import { RegisterData, registerSchema } from "@/schemas/auth/registerSchema";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link } from "@tanstack/react-router";
import { useForm } from "react-hook-form";

export function RegisterForm() {
  const form = useForm<RegisterData>({
    resolver: zodResolver(registerSchema),
  });

  const onSubmit = (data: RegisterData) => {
    console.log(data);
  };

  return (
    <Card className="mx-auto max-w-sm">
      <CardHeader>
        <CardTitle className="text-2xl">Register</CardTitle>
        <CardDescription>
          This is a simple PoC of a payment gateway implementation.
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
              <FormItem>
                <FormLabel htmlFor="confirm-password">
                  Confirm password
                </FormLabel>
                <FormControl>
                  <Input
                    id="confirm-password"
                    type="password"
                    required
                    {...form.register("confirmPassword")}
                  />
                </FormControl>
                <FormDescription />
                <FormMessage>
                  {form.formState.errors.confirmPassword?.message}
                </FormMessage>
              </FormItem>
              <Button type="submit" className="w-full">
                Register
              </Button>
            </div>
            <div className="mt-4 text-center text-sm">
              Already have an account?{" "}
              <Link to="/auth/login" className="underline">
                Log in
              </Link>
            </div>
          </form>
        </Form>
      </CardContent>
    </Card>
  );
}
