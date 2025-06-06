import { useEffect, useState } from 'react';
import Swal from 'sweetalert2'; 
import { API_BASE_URL } from '../apiConfig';
import { useSignalR } from "../SignalRProvider";
import { useNavigate } from 'react-router-dom';

const registerUrl = `${API_BASE_URL}users-api/api/accounts/register`;

const Register = () => {
  const { connection, connectionId } = useSignalR();
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    nickName: '',
    email: '',
    password: '',
    passwordConfirm: '',
    connectionId: connectionId
  });

  const [errors, setErrors] = useState({});

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const response = await fetch(registerUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData),
      });
      if (response.ok) {
          return;
      } else {
        const error = await response.json();
        if (response.status === 409) {
          Swal.fire({
            icon: 'error',
            title: 'Conflict',
            text: error.description,
          });
        } else {
          const validationErrors = {};

          for (const field in error.errors) {
            if (error.errors[field] && error.errors[field].length > 0) {
              validationErrors[field] = error.errors[field][0];
            }
          }

          setErrors(validationErrors);
        
          if (validationErrors["ConnectionId"]){
            Swal.fire({
              icon: 'error',
              title: 'Server Error',
              text: 'An internal server error occurred. Please try again later.',
            });      
          }
        }
      }
    } catch (error) {
      Swal.fire({
        icon: 'error',
        title: 'Server Error',
        text: 'An internal server error occurred. Please try again later.',
      });
    }
  };

  useEffect(() => {
    if (connectionId) {
      setFormData(prev => ({ ...prev, connectionId }));
    }
  }, [connectionId]);

  useEffect(() => {
    if(!connection) return;
    connection.on("NotifyUserRegistered", () => {
      setErrors({});
      navigate("/confirm-email", { state: { email: formData.email } })
    });
  }, [connection, formData.email]);

  return (
    <div className="container mt-5" style={{ maxWidth: '400px' }}>
      <h2 className="text-center mb-4 text-light">Register</h2>
      <form onSubmit={handleSubmit} className="p-4 border rounded-3 bg-dark shadow-sm">
        <div className="mb-3">
          <label htmlFor="nickName" className="form-label text-light">Nickname</label>
          <input
            type="text"
            id="nickName"
            name="nickName"
            className={`form-control ${errors['NickName'] ? 'is-invalid' : 'bg-secondary text-light border-0'}`}
            value={formData.nickName}
            onChange={handleChange}
            style={{ backgroundColor: '#343a40' }}
          />
          {errors['NickName'] && <div className="invalid-feedback">{errors['NickName']}</div>}
        </div>
  
        <div className="mb-3">
          <label htmlFor="email" className="form-label text-light">Email</label>
          <input
            type="email"
            id="email"
            name="email"
            className={`form-control ${errors['Email'] ? 'is-invalid' : 'bg-secondary text-light border-0'}`}
            value={formData.email}
            onChange={handleChange}
            style={{ backgroundColor: '#343a40' }}
          />
          {errors['Email'] && <div className="invalid-feedback">{errors['Email']}</div>}
        </div>
  
        <div className="mb-3">
          <label htmlFor="password" className="form-label text-light">Password</label>
          <input
            type="password"
            id="password"
            name="password"
            className={`form-control ${errors['Password'] ? 'is-invalid' : 'bg-secondary text-light border-0'}`}
            value={formData.password}
            onChange={handleChange}
            style={{ backgroundColor: '#343a40' }}
          />
          {errors['Password'] && <div className="invalid-feedback">{errors['Password']}</div>}
        </div>
  
        <div className="mb-3">
          <label htmlFor="passwordConfirm" className="form-label text-light">Confirm Password</label>
          <input
            type="password"
            id="passwordConfirm"
            name="passwordConfirm"
            className={`form-control ${errors['PasswordConfirm'] ? 'is-invalid' : 'bg-secondary text-light border-0'}`}
            value={formData.passwordConfirm}
            onChange={handleChange}
            style={{ backgroundColor: '#343a40' }}
          />
          {errors['PasswordConfirm'] && <div className="invalid-feedback">{errors['PasswordConfirm']}</div>}
        </div>
  
        <button type="submit" className="btn btn-primary w-100">Register</button>
      </form>
    </div>
  );  
};

export default Register