import { useNavigate, Outlet, useLocation } from "react-router-dom";
import { useAuth } from "../AuthProvider";
import styles from "../Styles/Profile.module.css";

const Profile = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const navItems = [
    { label: "Own data", path: "/profile/avatar", icon: "bi-person" },
    { label: "System Logs", path: "/profile/logs", icon: "bi-journal-text" },
  ];

  return (
    <div className={styles.container}>
      <aside className={styles.sidebar}>
        <nav className={styles.navList}>
          {navItems.map((item) => {
            const isActive = location.pathname === item.path;
            return (
              <button
                key={item.path}
                className={`${styles.navItem} ${isActive ? styles.navItemActive : ""}`}
                onClick={() => navigate(item.path)}
              >
                <i className={`bi ${item.icon}`}></i>
                {item.label}
              </button>
            );
          })}
        </nav>

        <button className={styles.logoutButton} onClick={handleLogout}>
          <i className="bi bi-box-arrow-right"></i> Log out
        </button>
      </aside>

      <main className={styles.content}>
        <Outlet />
      </main>
    </div>
  );
};

export default Profile;