import { useEffect, useState } from 'react';
import './App.css';

function App() {
  const [data, setData] = useState('');
  const [signature, setSignature] = useState('');

  useEffect(() => {
    fetch('https://localhost:7146/api/payments/create', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        amount: 100,
        currency: 'UAH',
        description: 'Оплата товару',
        orderId: 'ORDER123',
        resultUrl: 'https://localhost:3000/payment-success'
      })
    })
      .then(res => res.json())
      .then(res => {
        setData(res.data);
        setSignature(res.signature);
      });
  }, []);

  return (
    <div className="App">
      <h1>Оплата через LiqPay</h1>
      {data && signature ? (
        <form
          method="POST"
          action="https://www.liqpay.ua/api/3/checkout"
          acceptCharset="utf-8"
        >
          <input type="hidden" name="data" value={data} />
          <input type="hidden" name="signature" value={signature} />
          <button type="submit">Перейти до оплати</button>
        </form>
      ) : (
        <p>Завантаження даних платежу...</p>
      )}
    </div>
  );
}

export default App;

