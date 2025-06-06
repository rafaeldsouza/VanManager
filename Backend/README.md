# Sistema de Gestão de Transporte Escolar

Este projeto oferece uma solução completa para o gerenciamento de transporte escolar por vans. Permite o cadastro e organização de rotas, alunos, vans e motoristas, além de recursos de notificação e geolocalização em tempo real para os pais.

## Principais Funcionalidades

- Cadastro de rotas com embarque e desembarque personalizados por aluno
- Controle de presença e ausências (pontuais e programadas)
- Associação de múltiplos responsáveis por aluno
- Compartilhamento de localização em tempo real durante o trajeto
- Notificações automáticas de embarque, desembarque e proximidade
- Histórico completo de viagens e pontos de parada
- Gestão de planos comerciais com visibilidade controlada

A solução foi pensada para oferecer praticidade ao dono da frota, eficiência ao motorista e transparência total aos pais.

---

## Tecnologias Utilizadas

- **.NET 8** (C#)
- **Entity Framework Core**
- **MediatR** (CQRS)
- **Docker** (containerização)
- **PostgreSQL** (banco de dados relacional)
- **Seq** (logs estruturados)
- **Swagger** (documentação de API)
- **JWT** (autenticação)
- **xUnit** (testes unitários)
- **AutoMapper** (mapeamento de objetos)

---

## Organização do Projeto

```
VanManager/
├── src/
│   ├── VanManager.API/           # Camada de apresentação (controllers, endpoints)
│   ├── VanManager.Application/   # Casos de uso, comandos, queries, interfaces
│   ├── VanManager.Domain/        # Entidades de domínio e regras de negócio
│   ├── VanManager.Infrastructure/# Persistência, serviços externos, implementações
├── tests/
│   ├── VanManager.UnitTests/     # Testes unitários
│   ├── VanManager.IntegrationTests/ # Testes de integração
├── docker-compose.yml            # Orquestração dos containers
├── README.md
```

---

## Como Rodar o Projeto

### 1. Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop/)
- [Git](https://git-scm.com/)

### 2. Clonar o Repositório

```sh
git clone https://github.com/seu-usuario/van-manager.git
cd van-manager/Backend
```

### 3. Configurar Variáveis de Ambiente

Crie um arquivo `.env` na raiz do projeto `Backend` com as seguintes variáveis (ajuste conforme necessário):

```
JWT_SECRET=suachavesecreta
SMTP_SERVER=smtp.seuprovedor.com
SMTP_PORT=587
SMTP_USERNAME=usuario
SMTP_PASSWORD=senha
APP_URL=http://localhost:8080
```

### 4. Subir os Containers (API, PostgreSQL e Seq)

```sh
docker-compose up -d
```

- A API estará disponível em `http://localhost:8080`
- O banco PostgreSQL estará em `localhost:5432`
- O Seq (logs) estará em `http://localhost:5341`

### 5. Aplicar Migrations e Inicializar o Banco

Com os containers rodando, aplique as migrations do Entity Framework Core:

```sh
docker exec -it <nome_do_container_api> bash
dotnet ef database update --project ../VanManager.Infrastructure --startup-project .
exit
```

Para criar uma nova migration:

```sh
dotnet ef migrations add NomeDaMigration --project src/VanManager.Infrastructure --startup-project src/VanManager.API
```

### 6. Acessar a Documentação da API

Acesse `http://localhost:8080/swagger` para visualizar e testar os endpoints.

---

## Testes

Para rodar os testes unitários e de integração:

```sh
dotnet test
```

---

## Contribuição

Sinta-se à vontade para abrir issues e pull requests!

---

## Licença

Este projeto está sob a licença MIT.