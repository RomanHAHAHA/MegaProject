import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import Menu from "./components/Menu";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Products from "./pages/Products";
import Profile from "./pages/Profile";
import CreateProductForm from "./forms/CreateProductForm";
import ConfirmEmailWrapper from "./components/ConfirmEmailWrapper";
import CreateOrderForm from "./forms/CreateOrderForm";

function App() {
  return (
    <div>
      <Menu />
      <div className="container mt-5 pt-4">
        <Routes>
          <Route path="/" element={<Products />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/profile" element={<Profile />} />
          <Route path="/create-product" element={<CreateProductForm />} />
          <Route path="/confirm-email" element={<ConfirmEmailWrapper />} />   
          <Route path="/create-order" element={<CreateOrderForm />} />            
        </Routes>
      </div>
    </div>
  );
}

export default App;

