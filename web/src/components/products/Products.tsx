import { useQuery } from "@tanstack/react-query";
import Product from "./Product";
import fetchProducts from "@/services/product/fetchProducts";

export default function Products() {
  const { data } = useQuery({
    queryKey: ["products"],
    queryFn: fetchProducts,
  });
  console.log(data);
  return (
    <div className="flex gap-5">
      {data?.map((e) => {
        return <Product {...e} key={e.id} />;
      })}
    </div>
  );
}
