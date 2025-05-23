# Sistema de Cadastro de Clientes

Solução completa para cadastro de clientes com validações, processamento assíncrono e notificações por e-mail.

## Funcionalidades

Este projeto consiste em uma solução completa para cadastro de clientes, composta por:

- **WebAPI**: API RESTful para gerenciamento de cadastros
- **WebApplication**: Interface web para cadastro de clientes
- **EmailService**: Serviço para envio de e-mails de notificação

## Instalação

Para instalar e executar o projeto, siga os passos abaixo:

1. Crie um arquivo `.env` na raiz do projeto com as seguintes variáveis:

```env

# SendGrid
SENDGRID_API_KEY=sua_chave_aqui
```

2. Clone o repositório:
   ```bash
   git clone https://github.com/caroline-nunes-pathbit/Projeto02.git
   cd Projeto02
   ```

3. Construa e inicie os containers usando Docker:
   ```bash
   docker-compose up -d --build
   ```

## Uso
Após iniciar a aplicação, acesse `http://localhost:9090` em seu navegador.

## Tecnologias

- **Backend**: .NET 8.0
- **Frontend**: React
- **Banco de Dados**: MongoDB
- **Mensageria**: RabbitMQ
- **Envio de E-mails**: SendGrid
- **Consulta de CEP**: https://ceprapido.com
- **Contêinerização**: Docker

## Fluxo do Cadastro

1. **Dados Básicos**
   - Nome
   - Data de Nascimento
   - CPF
   - E-mail
   - Telefone

2. **Dados Financeiros**
   - Renda
   - Patrimônio
   - Validação: Renda + Patrimônio > R$ 1.000,00

3. **Endereço**
   - CEP (consulta automática via https://ceprapido.com)
   - Rua
   - Número
   - Bairro
   - Cidade
   - Estado

4. **Segurança**
   - Senha
   - Confirmação de Senha

## Notificações por E-mail

Após o cadastro, o cliente receberá um e-mail com a seguinte mensagem:

```
Olá [Nome do Cliente],

O seu cadastro está em análise e em breve você receberá um e-mail com novas atualizações sobre seu cadastro.

Atenciosamente,
Equipe PATHBIT
```
