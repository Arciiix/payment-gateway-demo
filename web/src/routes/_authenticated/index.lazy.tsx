import { NavUser } from "@/components/blocks/NavUser";
import Products from "@/components/products/Products";
import BillingTable from "@/components/billing/BillingTable";
import { createLazyFileRoute } from "@tanstack/react-router";
import { CreateProduct } from "@/components/products/CreateProduct";

export const Route = createLazyFileRoute("/_authenticated/")({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <div className="hidden h-full flex-1 flex-col space-y-8 p-8 md:flex">
      <div className="flex items-center justify-between space-y-2">
        <div>
          <h2 className="text-2xl font-bold tracking-tight">Welcome back!</h2>
          <p className="text-muted-foreground">
            Here&apos;s the billing information
          </p>
        </div>
        <div className="flex items-center space-x-2">
          <NavUser />
          <CreateProduct />
        </div>
      </div>

      <Products />

      <BillingTable />
    </div>
  );
}
