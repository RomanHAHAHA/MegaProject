import { useEffect, useRef } from "react";
import { useCart } from "../hooks/useCart";
import { API_BASE_URL } from "../apiConfig";
import { Trash2, Plus, Minus } from "lucide-react"; 
import imagePlaceholder from "../asserts/imagePlaceholder.jpg";
import { Link } from "react-router-dom";

const imageUrl = `${API_BASE_URL}product-images/`;

const CartPopup = ({ onClose }) => {
  const { cartItems, totalPrice, updateQuantity, removeItem } = useCart();
  const popupRef = useRef(null);

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (popupRef.current && !popupRef.current.contains(event.target)) {
        onClose();
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [onClose]);

  return (
    <div
      ref={popupRef}
      className="position-absolute top-100 end-0 mt-2 bg-dark text-white border rounded shadow"
      style={{ width: "320px", zIndex: 999 }}
    >
      <div className="p-3 d-flex justify-content-between align-items-center border-bottom">
        <strong>Cart</strong>
        <button className="btn-close btn-close-white" onClick={onClose}></button>
      </div>
      <div className="p-3">
        {cartItems.length === 0 ? (
          <p className="text-white">Cart is empty.</p>
        ) : (
          <>
            {cartItems.map((item) => (
              <div key={item.product.id} className="d-flex align-items-center mb-3">
                <img
                  src={item.product.mainImagePath
                    ? `${imageUrl}${item.product.mainImagePath}`
                    : imagePlaceholder}
                  alt={item.product.name}
                  style={{
                    width: "60px",
                    height: "60px",
                    objectFit: "cover",
                  }}
                  className="me-3 rounded"
                />
                <div className="flex-grow-1">
                  <div className="small fw-bold text-white">
                    {item.product.name}
                  </div>
                  <div className="d-flex align-items-center mt-1 mb-1">
                    <button
                      type="button"
                      className="btn btn-light p-0 d-flex justify-content-center align-items-center"
                      style={{ width: "24px", height: "24px", borderRadius: "4px" }}
                      onClick={() => updateQuantity(item.product.id, "decrement")}
                      disabled={item.quantity === 1}
                    >
                      <Minus size={16} />
                    </button>
                    <span className="mx-2 text-white fw-bold">{item.quantity}</span>
                    <button
                      type="button"
                      className="btn btn-light p-0 d-flex justify-content-center align-items-center"
                      style={{ width: "24px", height: "24px", borderRadius: "4px" }}
                      onClick={() => updateQuantity(item.product.id, "increment")}
                    >
                      <Plus size={16} />
                    </button>
                  </div>
                  <div className="small text-white opacity-75">
                    {item.quantity} x {item.product.price.toFixed(2)} UAH
                  </div>
                </div>
                <button
                  type="button"
                  className="btn btn-danger p-0 d-flex justify-content-center align-items-center ms-3"
                  style={{ width: "28px", height: "28px", borderRadius: "4px" }}
                  onClick={() => removeItem(item.product.id)}
                >
                  <Trash2 size={16} color="white" />
                </button>
              </div>
            ))}
            <div className="mt-3 text-end fw-bold text-white fs-6">
              Total: {totalPrice.toFixed(2)} UAH
            </div>
          </>
        )}
      </div>
      <div className="px-3 pb-3">
        <Link to="/create-order" className="btn btn-light w-100 fw-bold" onClick={onClose}>
          Create Order
        </Link>
      </div>
    </div>
  );
};

export default CartPopup;
