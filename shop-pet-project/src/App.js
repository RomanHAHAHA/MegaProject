import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import Menu from "./components/Menu";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Products from "./pages/Products";
import Profile from "./pages/Profile";
import ConfirmEmailWrapper from "./components/ConfirmEmailWrapper";
import CreateOrderForm from "./forms/CreateOrderForm";
import ChangeAvatar from "./pages/ChangeAvatar";
import Logs from "./pages/Logs";
import ConfirmedOrders from "./pages/ConfirmedOrders";
import ProductPage from "./pages/ProductPage";
import CreateProductPage from "./pages/CreateProductPage";
import AddCategoriesPage from "./pages/AddCategoriesPage";
import AddImagesPage from "./pages/AddImagesPage";
import AddCharacteristicsPage from "./pages/AddCharacteristicsPage";
import UpdateProductPage from "./pages/product/UpdateProductPage";
import MyOrdersPage from "./pages/order/MyOrdersPage";
import CategoriesAdminPage from "./pages/category/Categories";

function App() {
  return (
    <div>
      <Menu />
      <div className="container mt-5 pt-4">
        <Routes>
          <Route path="/" element={<Products />} />
          <Route path="/create-product" element={<CreateProductPage />} />
          <Route path="/products/:productId/update" element={<UpdateProductPage />} />
          <Route path="/products/:productId/add-categories" element={<AddCategoriesPage />} />
          <Route path="/products/:productId/add-images" element={<AddImagesPage />} />
          <Route path="/products/:productId/add-characteristics" element={<AddCharacteristicsPage />} />
          <Route path="/products/:productId" element={<ProductPage />} />

          <Route path="categories" element={<CategoriesAdminPage/>} />
          
          <Route path="/orders" element={<ConfirmedOrders />} />
          <Route path="/create-order" element={<CreateOrderForm />} />
          
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/confirm-email" element={<ConfirmEmailWrapper />} />   
          <Route path="/profile" element={<Profile />}>
            <Route path="avatar" element={<ChangeAvatar />} />
            <Route path="logs" element={<Logs />} />
            <Route path="my-orders" element={<MyOrdersPage/>}/>
          </Route>

        </Routes>
      </div>
    </div>
  );
}

export default App;

