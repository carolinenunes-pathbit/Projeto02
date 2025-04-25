import React, { useState } from 'react';
import BasicRegister from './BasicRegister';
import AddressRegister from './AddressRegister';
import FinancialRegister from './FinancialRegister';
import PasswordRegister from './PasswordRegister';
import AnalyzingData from './AnalyzingData';

function Register()
{
    const [basicData, setBasicData] = useState({});
    const [addressData, setAddressData] = useState({});
    const [financialData, setFinancialData] = useState({});
    const [passwordData, setPasswordData] = useState({});
    const [register, setRegister] = useState(1);

    const handleNextBasicRegister = async (data) => {
        setBasicData(data);
        setRegister(2);
    };
    const handleNextAddressRegister = async (data) => {
        setAddressData(data);
        setRegister(3);
    };
    const handleNextFinancialRegister = async (data) => {
        setFinancialData(data);
        setRegister(4);
    };
    const handleNextPasswordRegister = async (data) => {
        setPasswordData(data);
        setRegister(5);
    };
    const handleBack = () => {
        setRegister((previous) => previous -1);
    };

    return (
        <div>
            {register === 1 && <BasicRegister onNext={handleNextBasicRegister}/>}
            {register === 2 && <AddressRegister onNext={handleNextAddressRegister} onBack={handleBack}/>}
            {register === 3 && <FinancialRegister onNext={handleNextFinancialRegister} onBack={handleBack}/>}
            {register === 4 && <PasswordRegister onNext={handleNextPasswordRegister} onBack={handleBack}/>}
            {register === 5 && <AnalyzingData/>}
        </div>
    );
}

export default Register;