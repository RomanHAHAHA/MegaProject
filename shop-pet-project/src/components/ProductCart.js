import { API_BASE_URL } from "../apiConfig";
import imagePlaсeholder from '../asserts/imagePlaceholder.jpg';

const imagesUrl = `${API_BASE_URL}product-images/`;
const addToCartUrl = `${API_BASE_URL}carts-api/api/carts`

const ProductCard = ({ product }) => {
    const handleAddToCart = async () => {
        const response = await fetch(`${addToCartUrl}/${product.id}`, { method: 'POST', credentials: 'include' });
        if (response.ok) {

        }
    }

    return (
    <div className="col-12 col-sm-6 col-lg-3 mb-4">
      <div
        className="card h-100 bg-dark text-light border border-secondary rounded-4 shadow-sm overflow-hidden"
        style={{ transition: 'transform 0.2s ease-in-out' }}
      >
        <div
            className="d-flex align-items-center justify-content-center bg-transparent"
            style={{ height: "250px", overflow: "hidden" }}
            >
            <img
                src={
                product.mainImagePath
                    ? `${imagesUrl}${product.mainImagePath}`
                    : imagePlaсeholder
                }
                alt={product.name}
                style={{
                maxHeight: "100%",
                maxWidth: "80%",
                objectFit: "contain",
                }}
            />
            </div>
        <div className="card-body d-flex flex-column justify-content-between">
          <h5 className="card-title text-white">{product.name}</h5>
          <p className="card-text text-secondary small mb-2">
            {product.categories.join(", ")}
          </p>
          <div className="d-flex justify-content-between align-items-center mb-2">
            <span className="fw-bold text-light">{product.price.toFixed(2)} UAH</span>
            <span className="text-warning">⭐ {product.rating.toFixed(1)}</span>
          </div>
          <p className={`mb-3 ${product.isAvailable ? "text-success" : "text-danger"}`}>
            {product.isAvailable ? "In Stock" : "Out of Stock"}
          </p>
          <button
            className="btn btn-sm btn-primary w-100 rounded-3"
            onClick={handleAddToCart}
            disabled={!product.isAvailable}
          >
            Add to Cart
          </button>
        </div>
      </div>
    </div>
  );
};

export default ProductCard;
