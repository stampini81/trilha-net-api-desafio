# DIO - Trilha .NET - API e Entity Framework
www.dio.me

## Desafio de projeto
Para este desafio, você precisará usar seus conhecimentos adquiridos no módulo de API e Entity Framework, da trilha .NET da DIO.

## Contexto
Você precisa construir um sistema gerenciador de tarefas, onde você poderá cadastrar uma lista de tarefas que permitirá organizar melhor a sua rotina.

Essa lista de tarefas precisa ter um CRUD, ou seja, deverá permitir a você obter os registros, criar, salvar e deletar esses registros.

A sua aplicação deverá ser do tipo Web API ou MVC, fique a vontade para implementar a solução que achar mais adequado.

A sua classe principal, a classe de tarefa, deve ser a seguinte:

![Diagrama da classe Tarefa](diagrama.png)

Não se esqueça de gerar a sua migration para atualização no banco de dados.

## Métodos esperados
É esperado que você crie o seus métodos conforme a seguir:


**Swagger**


![Métodos Swagger](swagger.png)


**Endpoints**


| Verbo  | Endpoint                | Parâmetro | Body          |
|--------|-------------------------|-----------|---------------|
| GET    | /Tarefa/{id}            | id        | N/A           |
| PUT    | /Tarefa/{id}            | id        | Schema Tarefa |
| DELETE | /Tarefa/{id}            | id        | N/A           |
| GET    | /Tarefa/ObterTodos      | N/A       | N/A           |
| GET    | /Tarefa/ObterPorTitulo  | titulo    | N/A           |
| GET    | /Tarefa/ObterPorData    | data      | N/A           |
| GET    | /Tarefa/ObterPorStatus  | status    | N/A           |
| POST   | /Tarefa                 | N/A       | Schema Tarefa |

Esse é o schema (model) de Tarefa, utilizado para passar para os métodos que exigirem

```json
{
  "id": 0,
  "titulo": "string",
  "descricao": "string",
  "data": "2022-06-08T01:31:07.056Z",
  "status": "Pendente"
}
```


## Solução
O código está pela metade, e você deverá dar continuidade obedecendo as regras descritas acima, para que no final, tenhamos um programa funcional. Procure pela palavra comentada "TODO" no código, em seguida, implemente conforme as regras acima.

---

## Implementação realizada (esta solução)

Resumo do que foi implementado para atender aos requisitos do desafio:

- Plataforma e versões:
  - .NET 8 (TargetFramework net8.0) para compatibilidade com o ambiente.
  - Entity Framework Core 8.x (SqlServer e Sqlite disponíveis).
  - Swashbuckle (Swagger) habilitado no ambiente Development.

- Modelagem:
  - `Models/Tarefa.cs` com propriedades Id, Titulo, Descricao, Data, Status.
  - `Models/EnumStatusTarefa.cs` com valores `Pendente` e `Finalizado`.

- Persistência (EF Core):
  - `Context/OrganizadorContext.cs` (DbContext) com DbSet<Tarefa>.
  - `Context/DesignTimeOrganizadorContextFactory.cs` para comandos de migration.
  - Provedor de banco configurado dinamicamente em `Program.cs`:
    - Usa SQL Server se a connection string não for SQLite.
    - Fallback para SQLite local (`Data Source=organizador.db`) quando não houver SQL Server.
  - Inicialização do banco no startup: tenta aplicar migrations; se não houver, usa `EnsureCreated()` para o SQLite.

- Endpoints (CRUD) em `Controllers/TarefaController.cs`:
  - GET `/Tarefa/{id}`
  - PUT `/Tarefa/{id}`
  - DELETE `/Tarefa/{id}`
  - GET `/Tarefa/ObterTodos`
  - GET `/Tarefa/ObterPorTitulo?titulo=...`
  - GET `/Tarefa/ObterPorData?data=yyyy-MM-dd`
  - GET `/Tarefa/ObterPorStatus?status=Pendente|Finalizado`
  - POST `/Tarefa`

- Regras/validações aplicadas:
  - Data obrigatória no POST/PUT (não pode ser `DateTime.MinValue`).
  - Retornos apropriados: 404 quando não encontrado; 201 no POST; 204 no DELETE; 400 para entradas inválidas.

- Swagger:
  - Acessível em Development:
    - HTTP: `http://localhost:5181/swagger`
    - HTTPS: `https://localhost:7295/swagger` (pode requerer confiar o certificado dev)

- Migrations e banco:
  - Migration inicial (SQLite) criada: `Migrations/20250819174758_InitSqlite*.cs`.
  - Banco SQLite: arquivo `organizador.db` na raiz do projeto (quando em modo fallback).
  - Connection strings:
    - `appsettings.Development.json`: `ConexaoPadrao` = `Data Source=organizador.db`.
    - `appsettings.json`: inclui uma connection string de SQL Server como exemplo e fallback.

## Como rodar

1) Restaurar e compilar:
```powershell
dotnet restore
dotnet build
```

2) Rodar a API:
```powershell
dotnet run --project ".\TrilhaApiDesafio.csproj" -c Debug
```

3) Abrir o Swagger:
- `http://localhost:5181/swagger`
- Se preferir HTTPS, confie o certificado dev:
```powershell
dotnet dev-certs https --trust
```

## Comandos úteis (migrations)

Para recriar o banco local SQLite via migrations:
```powershell
dotnet ef migrations add InitSqlite -o Migrations
dotnet ef database update
```

Para usar SQL Server LocalDB (ou outro SQL Server), ajuste `ConexaoPadrao` em `appsettings.Development.json` e garanta que o servidor está acessível. Em seguida, gere nova migration se necessário.

## Teste rápido via PowerShell
```powershell
$body = @{ Titulo = 'Estudar'; Descricao = 'Estudar EF Core'; Data = '2025-08-20T10:00:00'; Status = 'Pendente' } | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri 'http://localhost:5181/Tarefa' -ContentType 'application/json' -Body $body
Invoke-RestMethod -Method Get -Uri 'http://localhost:5181/Tarefa/ObterTodos'
```

## Usando SQL Server (alternativa ao SQLite)

Este projeto detecta o provedor pelo formato da connection string (Program.cs). Para usar SQL Server:

1) Defina a connection string em `appsettings.Development.json` (ou em variável de ambiente `ConnectionStrings__ConexaoPadrao`):

Exemplos:
- LocalDB (dev):
```
Server=(localdb)\\MSSQLLocalDB;Database=OrganizadorTarefas;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```
- SQL Express (local):
```
Server=.\\SQLEXPRESS;Database=OrganizadorTarefas;Trusted_Connection=True;TrustServerCertificate=True
```
- SQL Server/Azure SQL (usuario/senha):
```
Server=tcp:<servidor>.database.windows.net,1433;Initial Catalog=OrganizadorTarefas;Persist Security Info=False;User ID=<usuario>;Password=<senha>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

2) Gere migrations específicas para SQL Server (recomendado recriar as migrations se antes usou SQLite):
```powershell
# Opcional: remover migrations anteriores e recriar
Remove-Item -Recurse -Force .\Migrations
dotnet ef migrations add InitSqlServer -o Migrations
dotnet ef database update
```

3) Rode a API normalmente:
```powershell
dotnet run --project .\TrilhaApiDesafio.csproj -c Debug
```

Observações:
- Não é necessário alterar código para trocar o provedor; o Program.cs já seleciona SQL Server quando a connection string não é SQLite (não contém "Data Source=").
- Migrations podem conter diferenças por provedor; por isso, ao trocar de SQLite para SQL Server, prefira recriar as migrations.

## Observações
- O projeto foi atualizado para .NET 8 para compatibilidade com o runtime disponível.
- O fallback para SQLite permite rodar sem depender de SQL Server local.
- Caso queira fixar apenas SQL Server (como no enunciado original), basta definir a connection string e rodar as migrations para esse provedor.