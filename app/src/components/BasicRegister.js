import React, { useState } from 'react';
import axios from 'axios';
import '../styles/style.css';
import { URL } from '../shared/constants';
import { ID } from '../shared/constants';

function BasicRegister({ onNext }) 
{
    const [name, setName] = useState('');
    const [birthDate, setBirthDate] = useState('');
    const [documentNumber, setDocumentNumber] = useState('');
    const [email, setEmail] = useState('');
    const [phoneNumber, setPhoneNumber] = useState('');

    const handleSubmit = async (event) => {
        event.preventDefault();

        try {
            const result = await axios.post(`${URL}/basic`, {
                name, 
                birthDate,
                documentNumber,
                email,
                phoneNumber
            });
            const id = result.data.id;
            console.log(result);
            localStorage.setItem(ID, id);
            onNext({ id, name, birthDate, documentNumber, email, phoneNumber });
        }
        catch(error) {
            console.error('Erro ao enviar dados: ', error);
        }
    };
    return (
        <form onSubmit={handleSubmit}>
            <label>Nome: <input type="text" value={name} onChange={(e) => setName(e.target.value)} required/></label>
            <label>Data de Nascimento: <input type="date" value={birthDate} onChange={(e) => setBirthDate(e.target.value)} required/></label>
            <label>CPF: <input type="text" value={documentNumber} onChange={(e) => setDocumentNumber(e.target.value)} required/></label>
            <label>Email: <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required/></label>
            <label>Telefone: <input type="text" value={phoneNumber} onChange={(e) => setPhoneNumber(e.target.value)} required/></label>
            <button type="submit">Próximo</button>
        </form>
    );
}

export default BasicRegister;