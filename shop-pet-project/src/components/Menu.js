import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../AuthProvider";  
import imagePlaceholder from '../asserts/default_avatar_image.png';
import { API_BASE_URL } from "../apiConfig";
import { ShoppingCart } from "lucide-react"; 
import CartPopup from "../components/CartPopup";

const avatarUrl = `${API_BASE_URL}user-images/`;

const Menu = () => {
  const { user } = useAuth();  
  const [isAuthorized, setIsAuthorized] = useState(false);
  const [showCart, setShowCart] = useState(false);

  useEffect(() => {
    if (user) {
      setIsAuthorized(true);
    }
  }, [user]);

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark fixed-top">
      <div className="container">
        <Link className="navbar-brand" to="/">OLX Killer</Link>
        <div className="collapse navbar-collapse" id="navbarNav">
          <ul className="navbar-nav ms-auto align-items-center">
            <li className="nav-item position-relative">
              <button
                className="btn btn-link nav-link p-0"
                onClick={() => setShowCart(prev => !prev)}
              >
                <ShoppingCart color="white" />
              </button>
              {showCart && <CartPopup onClose={() => setShowCart(false)} />}
            </li>
            {isAuthorized && user ? (
              <li className="nav-item d-flex align-items-center">
                <Link className="nav-link" to="/profile">{user.nickName}</Link>
                <img
                  src={user.avatarImageName ? `${avatarUrl}${user.avatarImageName}` : imagePlaceholder}
                  onError={(e) => { e.currentTarget.src = imagePlaceholder }}
                  alt="avatar"
                  className="rounded-circle me-2"
                  style={{ width: "32px", height: "32px", objectFit: "cover" }}
                />
              </li>
            ) : (
              <li className="nav-item">
                <Link className="nav-link" to="/login">Login</Link>
              </li>
            )}
            <li className="nav-item">
              <Link className="nav-link" to="/create-product">Create Product</Link>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  );
};

export default Menu;