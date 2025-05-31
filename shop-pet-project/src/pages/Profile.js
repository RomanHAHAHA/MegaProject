import { useNavigate } from "react-router-dom";
import { useAuth } from "../AuthProvider";

const Profile = () => {
    const navigate = useNavigate();
    const { logout } = useAuth();

    const handleLogout = async () => {
        logout();
        navigate("/login");
    }
    
    return (
        <div>
            <button onClick={handleLogout}>Logout</button>
        </div>
    );
}

export default Profile;