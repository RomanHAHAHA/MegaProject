import ReactDOM from 'react-dom/client';
import App from './App';
import 'bootstrap/dist/css/bootstrap.min.css';
import "./Styles/Index.css";
import { AuthProvider } from "./AuthProvider"; 
import { SignalRProvider } from './SignalRProvider';
import { BrowserRouter } from 'react-router-dom';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <BrowserRouter>
    <AuthProvider>
      <SignalRProvider>
        <App />
      </SignalRProvider> 
    </AuthProvider>
  </BrowserRouter>
);