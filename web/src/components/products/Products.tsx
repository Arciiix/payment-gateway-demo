import fetchProducts from "@/services/product/fetchProducts";
import { useQuery } from "@tanstack/react-query";
import Product from "./Product";
import ProductSkeleton from "./ProductSkeleton";

export default function Products() {
  const { data, isLoading } = useQuery({
    queryKey: ["products"],
    queryFn: fetchProducts,
  });
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
    <div className="flex gap-5">
      {data?.map((e) => {
        return <Product {...e} key={e.id} />;
      })}
    </div>
  );
}
