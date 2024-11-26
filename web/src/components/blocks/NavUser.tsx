"use client";

import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { userQuery } from "@/queries/auth/user";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Button } from "../ui/button";
import logOutUser from "@/services/auth/logOutUser";
import { toast } from "sonner";
import { useNavigate } from "@tanstack/react-router";
import { RefreshCwIcon } from "lucide-react";
import fetchProductsRefreshed from "@/services/product/fetchProductsRefreshed";
import { productsQuery } from "@/queries/products/products";

export function NavUser() {
  const { data: user } = useQuery(userQuery);

  const navigate = useNavigate();

  const queryClient = useQueryClient();
  const { mutate: logOut, isPending: isPendingLogOut } = useMutation({
    mutationFn: logOutUser,
    onSuccess: () => {
      navigate({ to: "/auth/login" });
      queryClient.setQueryData(userQuery.queryKey, null);

      toast.success("Logged out!");
    },
  });

  const { mutate: refreshData, isPending: isPendingData } = useMutation({
    mutationFn: fetchProductsRefreshed,
    onSuccess: (data) => {
      queryClient.setQueryData(productsQuery.queryKey, data);
    },
  });

  if (!user) return null;
  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="ghost" className="relative h-8 w-8 rounded-full">
          <Avatar className="h-9 w-9">
            {/* <AvatarImage src="/avatars/03.png" alt="@shadcn" /> */}
            <AvatarFallback>{user?.email.slice(0, 2)}</AvatarFallback>
          </Avatar>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent className="w-56" align="end" forceMount>
        <DropdownMenuLabel className="font-normal">
          <div className="flex flex-col space-y-1">
            <p className="text-sm font-medium leading-none">{user!.email}</p>
          </div>
        </DropdownMenuLabel>

        <DropdownMenuSeparator />
        <DropdownMenuItem
          onClick={() => refreshData()}
          disabled={isPendingData}
        >
          <RefreshCwIcon /> Refresh data
        </DropdownMenuItem>
        <DropdownMenuSeparator />
        <DropdownMenuItem onClick={() => logOut()} disabled={isPendingLogOut}>
          Log out
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
