Aqui está o markdown sem formatação:

# API de Gerenciamento de Despesas

Esta API gerencia despesas através de um CRUD de *Expenses* com autenticação via OAuth 2.0 (Google). Ela também permite listar categorias de despesas.

## Configuração

1. Clone o repositório:

```bash

   git clone https://github.com/EduardoKroetz/ExpenseAPI.git  
   cd ExpenseTracker
```

2. Configure o arquivo `appsettings.json` com suas credenciais:

```bash

   {
     "Google": {
       "ClientId": "seu-google-client-id",
       "ClientSecret": "seu-google-client-secret"
     },
     "Jwt": {
       "Key": "sua-chave-jwt-secreta"
     },
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=expense;Username=postgres;Password=expense-admin"
     }
   }
```

3. Execute o banco de dados PostgreSQL localmente e crie as tabelas com o seguinte SQL:

```bash

   CREATE TABLE "Users"  
   (  
       "Id" UUID PRIMARY KEY,  
       "Name" TEXT,  
       "Email" TEXT  
   );

   CREATE TABLE "Categories"  
   (  
       "Id" UUID PRIMARY KEY,  
       "Name" text  
   );

   CREATE TABLE "Expenses"  
   (  
       "Id" UUID PRIMARY KEY,  
       "Name" TEXT,  
       "UserId" UUID,  
       "CategoryId" UUID,  
       "CreatedAt" timestamp with time zone,  

       FOREIGN KEY ("CategoryId") REFERENCES "Categories"("Id"),  
       FOREIGN KEY ("UserId") REFERENCES "Users"("Id")  
   );

   INSERT INTO Categories(Id, Name) VALUES   
       (gen_random_uuid(), 'Mantimentos'),  
       (gen_random_uuid(), 'Lazer'),   
       (gen_random_uuid(), 'Eletrônica'),   
       (gen_random_uuid(), 'Utilidades'),   
       (gen_random_uuid(), 'Vestuário'),   
       (gen_random_uuid(), 'Saúde'),   
       (gen_random_uuid(), 'Diversos');

```

4. Execute a aplicação:

```bash

   dotnet run
```

## Endpoints

### Autenticação

- **POST /auth/google**: Autenticação via OAuth 2.0 com Google.

### Despesas

- **GET /expenses**: Lista todas as despesas.  
- **POST /expenses**: Cria uma nova despesa.  
- **PUT /expenses/{id}**: Atualiza uma despesa existente.  
- **DELETE /expenses/{id}**: Remove uma despesa.
- **GET /expenses?filter=**: Filtrar as despesas (filtros: week, month, three-months)

### Categorias

- **GET /categories**: Lista todas as categorias.
