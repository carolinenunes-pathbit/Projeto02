import React from 'react';
import '../styles/analyzing.css';
function AnalyzingData() {
  return (
    <div className="loading-container">
      <div className="spinner"></div>
      <p className="loading-text">Analisando dados. Por favor, aguarde...</p>
    </div>
  );
}

export default AnalyzingData;