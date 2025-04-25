import React, {useState} from 'react';
import axios from 'axios';
import '../styles/style.css';
import { URL } from '../shared/constants';
import { ID } from '../shared/constants';

function FinancialRegister({ onNext })
{
    const customerId = localStorage.getItem(ID);
    const [rent, setRent] = useState('');
    const [financialAssets, setFinancialAssets] = useState('');

    const handleSubmit = async (event) => {
        event.preventDefault();

        try {
            await axios.patch(`${URL}/financial/{id}`, {
                id: customerId,
                rent,
                financialAssets
            });
            onNext({ id: customerId, rent, financialAssets });
        }
        catch (error) {
            console.error('Erro ao enviar dados: ', error);
        }
    };
    return (
        <form onSubmit={handleSubmit}>
            <label>Renda: <input type="text" value={rent} onChange={(e) => setRent(e.target.value)} required/></label>
            <label>Patrimônio: <input type="text" value={financialAssets} onChange={(e) => setFinancialAssets(e.target.value)} required/></label>
            <button>Voltar</button>
            <button type="submit">Próximo</button>
        </form>
    )
}

export default FinancialRegister;