import { productsQuery } from "@/queries/products/products";
import { useQuery } from "@tanstack/react-query";
import Product from "./Product";
import ProductSkeleton from "./ProductSkeleton";

export default function Products() {
  const { data, isLoading } = useQuery(productsQuery);
  console.log(data);
  if (isLoading) {
    return (
      <div className="flex gap-5">
        <ProductSkeleton />
        <ProductSkeleton />
        <ProductSkeleton />
        <ProductSkeleton />
      </div>
    );
  }
  return (
    <div className="grid grid-cols-1 lg:grid-cols-2 w-max mx-auto">
      {data?.map((e) => {
        return <Product {...e} key={e.id} />;
      })}
    </div>
  );
}
