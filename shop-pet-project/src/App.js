import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import Menu from "./components/Menu";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Products from "./pages/Products";
import Profile from "./pages/Profile";
import CreateProductPage from "./pages/CreateProductPage";
import ConfirmEmailWrapper from "./components/ConfirmEmailWrapper";
import CreateOrderForm from "./forms/CreateOrderForm";
import ChangeAvatar from "./pages/ChangeAvatar";
import Logs from "./pages/Logs";
import ConfirmedOrders from "./pages/ConfirmedOrders";

function App() {
  return (
    <div>
      <Menu />
      <div className="container mt-5 pt-4">
        <Routes>
          <Route path="/" element={<Products />} />
          <Route path="/orders" element={<ConfirmedOrders />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/profile" element={<Profile />}>
              {/* <Route path="details" element={<ProfileDetails />} />
              <Route path="password" element={<ChangePassword />} /> */}
              <Route path="avatar" element={<ChangeAvatar />} />
              <Route path="logs" element={<Logs />} />
          </Route>
          <Route path="/create-product" element={<CreateProductPage />} />
          <Route path="/confirm-email" element={<ConfirmEmailWrapper />} />   
          <Route path="/create-order" element={<CreateOrderForm />} />            
        </Routes>
      </div>
    </div>
  );
}

export default App;

