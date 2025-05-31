import { useEffect, useState } from "react"
import { API_BASE_URL } from "../apiConfig";
import ProductCard from "../components/ProductCart";

const productsUrl = `${API_BASE_URL}products-api/api/products`;

const Products = () => {
    const [products, setProducts] = useState([]);

    useEffect(() => {
        fetchProducts();
    }, []);

    const fetchProducts = async () => {
        console.log("fetch products")
        const response = await fetch(productsUrl, { method: 'GET' });
        if (response.ok) {
            const paginationData = await response.json();
            setProducts(paginationData.items);
        }
    }

    return (
        <div className="container mt-4">
            <div className="row">
                {products.map(product => (
                    <ProductCard key={product.id} product={product} />
                ))}
            </div>
        </div>
    );
}

export default Products;