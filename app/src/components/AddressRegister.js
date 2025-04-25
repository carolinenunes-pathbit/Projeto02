import React, { useState } from 'react';
import axios from 'axios';
import '../styles/style.css';
import { URL } from '../shared/constants';
import { ID } from '../shared/constants';

function AddressRegister({ onNext })
{
    const customerId = localStorage.getItem(ID);
    const [zipCode, setZipCode] = useState('');
    const [number, setNumber] = useState('');
    const [street, setStreet] = useState('');
    const [district, setDistrict] = useState('');
    const [city, setCity] = useState('');
    const [state, setState] = useState('');
    const [isDisabled, setIsDisabled] = useState(true);

    const handleSubmit = async (event) => {
        event.preventDefault();

        try {
            await axios.patch(`${URL}/address/{id}`, {
                id: customerId,
                zipCode,
                number,
                street,
                district,
                city,
                state
            });
            onNext({ zipCode, number, street, district, city, state });
        }
        catch (error) {
            console.error('Erro ao enviar dados: ', error);
        }
    }

    const handleCEPSearch = async () => {
        if(zipCode) {
            try {
                const response = await axios.get(`/api/addresses/${zipCode}`);
                if(response.data) {
                    const data = response.data[0];

                    setStreet(data.addressName || '');
                    setDistrict(data.districtName || '');
                    setCity(data.cityName || '');
                    setState(data.stateName || '');
                    setIsDisabled(true);
                }
            }
            catch (error) {
                console.error('Erro ao buscar CEP: ', error);
            }
        }
    };
    return (
        <form onSubmit={handleSubmit}>
            <label>CEP: <input type="text" value={zipCode} onChange={(e) => setZipCode(e.target.value)} required/></label>
            <label>Número: <input type="text" value={number} onChange={(e) => setNumber(e.target.value)} required/></label>
            <button type="button" onClick={handleCEPSearch}>Buscar</button>
            <label>Rua: <input type="text" value={street} disabled={isDisabled} required/></label>
            <label>Bairro: <input type="text" value={district} disabled={isDisabled} required/></label>
            <label>Cidade: <input type="text" value={city} disabled={isDisabled} required/></label>
            <label>Estado: <input type="text" value={state} disabled={isDisabled} required/></label>
            <button>Voltar</button>
            <button type="submit">Próximo</button>
        </form>
    )
}

export default AddressRegister;