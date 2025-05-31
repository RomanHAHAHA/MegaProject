import Swal from "sweetalert2";
import { API_BASE_URL } from "../apiConfig";
import { useState } from "react";

const createProductUrl = `${API_BASE_URL}products-api/api/products`;

const CreateProductForm = () => {
    const initialFormState = {
        name: '',
        description: '',
        price: '',
        stockQuantity: ''
    };
    const [formData, setFormData] = useState(initialFormState);
    const [validationErrors, setValidationErrors] = useState({});

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData((prev) => ({ ...prev, [name]: value }));
    };

    const createProduct = async (e) => {
        e.preventDefault();

        const productDto = {
            name: formData.name,
            description: formData.description,
            price: formData.price ? parseFloat(formData.price, 10) : null,
            stockQuantity: formData.stockQuantity ? parseInt(formData.stockQuantity, 10) : null
        };
        
        try 
        {
            const response = await fetch(createProductUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include',
                body: JSON.stringify(productDto) 
            });

            if (response.ok) {
                setFormData(initialFormState);
                setValidationErrors([]);
                Swal.fire({
                    title: 'Success',
                    text: 'Product created successfully',
                    icon: 'success'
                });
            } else if (response.status === 400){
                const data = await response.json();
                if (data.errors) {
                    setValidationErrors(data.errors);
                } else {
                    Swal.fire({
                        title: 'Error',
                        text: 'Failed to create product',
                        icon: 'error'
                    });
                }
            }
        } catch (error) {
            Swal.fire({
                title: 'Error',
                text: 'Unexpected error occured during the request',
                icon: 'error'
            });
        }
    }

    return (
        <div className="container mt-5" style={{ maxWidth: '900px' }}>
            <h2 className="text-center mb-4 text-light">Create Product</h2>
            <form onSubmit={createProduct} className="p-4 rounded-3 bg-dark shadow-sm">
                <div className="row">
                    <div className="col-md-6">
                        <div className="mb-3">
                            <label className="form-label text-light">Name</label>
                            <input
                                name="name"
                                className={`form-control bg-secondary text-light border-0 ${validationErrors['Name'] ? 'is-invalid' : ''}`}
                                value={formData.name}
                                onChange={handleChange}
                                style={{ backgroundColor: '#343a40' }}
                            />
                            {validationErrors['Name'] && <div className="invalid-feedback">{validationErrors['Name'][0]}</div>}
                        </div>

                        <div className="mb-3">
                            <label className="form-label text-light">Price</label>
                            <input
                                type="text"
                                name="price"
                                className={`form-control bg-secondary text-light border-0 ${validationErrors['Price'] ? 'is-invalid' : ''}`}
                                value={formData.price}
                                onChange={handleChange}
                                style={{ backgroundColor: '#343a40' }}
                            />
                            {validationErrors['Price'] && <div className="invalid-feedback">{validationErrors['Price'][0]}</div>}
                        </div>

                        <div className="mb-3">
                            <label className="form-label text-light">Stock Quantity</label>
                            <input
                                type="text"
                                name="stockQuantity"
                                className={`form-control bg-secondary text-light border-0 ${validationErrors['StockQuantity'] ? 'is-invalid' : ''}`}
                                value={formData.stockQuantity}
                                onChange={handleChange}
                                style={{ backgroundColor: '#343a40' }}
                            />
                            {validationErrors['StockQuantity'] && <div className="invalid-feedback">{validationErrors['StockQuantity'][0]}</div>}
                        </div>
                    </div>

                    <div className="col-md-6 d-flex flex-column">
                        <label className="form-label text-light">Description</label>
                        <textarea
                            name="description"
                            className={`form-control bg-secondary text-light border-0 flex-grow-1 ${validationErrors['Description'] ? 'is-invalid' : ''}`}
                            value={formData.description}
                            onChange={handleChange}
                            style={{
                                backgroundColor: '#343a40',
                                resize: 'none', }}
                        />
                        {validationErrors['Description'] && <div className="invalid-feedback">{validationErrors['Description'][0]}</div>}
                    </div>
                </div>

                <div className="mt-4">
                    <button type="submit" className="btn btn-primary w-100">Create</button>
                </div>
            </form>
        </div>
    );
}

export default CreateProductForm;