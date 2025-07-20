# TaskMasterPro

Gerenciador de tarefas ASP.NET Core MVC com autenticação de usuários.

## Funcionalidades

- Cadastro e login de usuários
- Criação, edição e exclusão de tarefas
- Marcar tarefas como concluídas
- Interface responsiva com Bootstrap
- Banco de dados local (SQLite) com Entity Framework Core

## Tecnologias

- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQLite
- Bootstrap 5

## Como rodar localmente

```bash
git clone https://github.com/dcair2024/TaskMasterPro.git
cd TaskMasterPro
dotnet restore
dotnet ef database update
dotnet run
