import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import Menu from "./components/Menu";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Products from "./pages/Products";
import Profile from "./pages/Profile";
import CreateProductForm from "./forms/CreateProductForm";

function App() {
  return (
    <Router>
        <div>
          <Menu />
          <div className="container mt-5 pt-4">
            <Routes>
              <Route path="/" element={<Products />} />
              <Route path="/login" element={<Login />} />
              <Route path="/register" element={<Register />} />
              <Route path="/profile" element={<Profile />} />
              <Route path="/create-product" element={<CreateProductForm />} />
            </Routes>
          </div>
        </div>
      </Router>
  );
}

export default App;

