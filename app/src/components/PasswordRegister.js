import React, { useState } from 'react';
import axios from 'axios';
import '../styles/style.css';
import { URL } from '../shared/constants';
import { ID } from '../shared/constants';

function PasswordRegister({ onNext })
{
    const customerId = localStorage.getItem(ID);
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');

    const handleSubmit = async (event) => {
        event.preventDefault();

        try {
            await axios.patch(`${URL}/password/{id}`, {
                id: customerId,
                password,
                confirmPassword
            });
            onNext({ id: customerId, password, confirmPassword });
        }
        catch (error) {
            console.error('Erro ao enviar dados: ', error);
        }
    };
    return (
        <form onSubmit={handleSubmit}>
            <label>Senha: <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} required/></label>
            <label>Confirmar senha: <input type="password" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)} required/></label>
            <button>Voltar</button>
            <button type="submit">Próximo</button>
        </form>
    )
}

export default PasswordRegister;