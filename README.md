# 🌙 Amêndoa's Dreams

> Diário digital de sonhos pessoal — Registre, organize e consulte seus sonhos com facilidade.

---

## 📖 Sobre o Projeto

**Amêndoa's Dreams** é uma aplicação web desenvolvida para registrar sonhos logo ao acordar. Mais do que um simples diário, este projeto foi concebido como um ambiente prático para aprender e aplicar **SQL**, **C# (.NET)** e **PHP** em conjunto, construindo algo real e com propósito pessoal.

A aplicação permite que o usuário mantenha um histórico completo de seus sonhos, incluindo título, descrição, data, horários, humor ao acordar e tags. Tudo isso com uma interface simples e uma arquitetura desacoplada, onde o frontend PHP consome uma API REST desenvolvida em C#.

---

## 🧱 Stack Tecnológica

| Camada                   | Tecnologia                                      | Observação                                                 |
|--------------------------|-------------------------------------------------|------------------------------------------------------------|
| **Banco de Dados**       | PostgreSQL                                      | Local no desenvolvimento; futuramente em AWS RDS ou Oracle |
| **Backend (API)**        | C# — ASP.NET Core Web API                       | .NET 8, REST API                                           |
| **Frontend** | PHP       | Interface web básica que consome a API via HTTP |
| **Comunicação**          | JSON (HTTP/REST)                                | Troca de dados entre frontend e backend                    |
| **ORM / Acesso a dados** | **Dapper**                                      | SQL puro e explícito, ideal para aprendizado               |

> **Por que Dapper em vez de Entity Framework?**  
> O EF gera SQL automaticamente, o que esconde o aprendizado. Com o Dapper, escrevemos SQL diretamente, tornando cada consulta clara e educativa — perfeito para dominar a linguagem na prática.

---

## 🏗️ Arquitetura Geral

```
[Navegador]
     🠗
[Frontend — PHP]          (Renderiza páginas HTML, envia formulários)
     🠗ㅤㅤ  HTTP + JSON
[API — C# ASP.NET Core]   (Processa as regras de negócio, valida dados)
     🠗
[PostgreSQL]              (Armazena todos os dados dos sonhos)
```

A comunicação entre o PHP e a API C# é feita via requisições HTTP (GET, POST, PUT, DELETE), trocando dados em formato JSON. Isso mantém as camadas independentes — o frontend não sabe nada sobre o banco de dados.

## ✅ Funcionalidades

### Parte 1

- [x] **Registrar um sonho** com:
  - Título
  - Descrição completa (relato do sonho)
  - Data do sonho
  - Horário estimado que dormiu
  - Horário estimado que acordou
  - Humor ao acordar (ex.: ansioso, feliz, confuso, assustado, neutro)
  - Tags (ex.: pesadelo, lúcido, recorrente, estranho)
- [x] **Listar** todos os sonhos (do mais recente ao mais antigo)
- [x] **Visualizar** o detalhe completo de um sonho
- [x] **Editar** um sonho já registrado
- [x] **Deletar** um sonho
- [x] **Seed data** — 5 sonhos fictícios pré-cadastrados para demonstração pública da aplicação (via `seed.sql`)

> ✅ Todos os endpoints implementados na API (Fase 2). Interface visual pendente (Fase 3 — PHP).

### Parte 2 — Melhorias

- [ ] Filtrar sonhos por **data** (ex.: "sonhos de maio de 2025")
- [ ] Filtrar sonhos por **tag**
- [ ] Filtrar sonhos por **humor**
- [ ] **Busca** por palavra-chave na descrição ou título

### Parte 3 — Futuro

- [ ] **Estatísticas** (quantidade de sonhos por mês, tags mais frequentes, humores mais comuns)
- [ ] **Login com Google** (OAuth 2.0) — cada usuário terá seus próprios sonhos privados
  - Adicionar tabela `users` ao banco de dados
  - Adicionar coluna `user_id` na tabela `dreams`
  - Autenticação via JWT entre o frontend PHP e a API C#
- [ ] **Migração do banco** para AWS RDS ou Oracle

---

## 🌱 Seed Data (Sonhos de Demonstração)

Para que qualquer pessoa possa testar a aplicação sem precisar cadastrar seus próprios sonhos, o banco virá com **5 sonhos fictícios pré-carregados** via script SQL (`seed.sql`).

| # | Título                            | Humor     | Tags                 |
|---|-----------------------------------|-----------|----------------------|
| 1 | O labirinto infinito              | ansioso   | recorrente, estranho |
| 2 | Voando sobre a cidade             | feliz     | lúcido               |
| 3 | A prova que eu esqueci de estudar | ansioso   | pesadelo, recorrente |
| 4 | O cachorro que falava             | confuso   | estranho             |
| 5 | Mergulhando no oceano de nuvens   | tranquilo | lúcido, estranho     |

> **Nota sobre a Parte 1 — Seed data:** Os sonhos de seed serão a base do Modo Demonstração. Usuários sem login verão esses sonhos carregados via sessão PHP — interagem normalmente, mas nenhuma alteração toca o banco. Quando o login for implementado, esses mesmos sonhos serão associados a um usuário fictício no banco para quem fizer login com Google.

---

## 🗃️ Modelo de Dados

### Tabela: `dreams`

| Coluna        | Tipo                      | Descrição                    |
|---------------|---------------------------|------------------------------|
| `id`          | `SERIAL PRIMARY KEY`      | Identificador único          |
| `title`       | `VARCHAR(255) NOT NULL`   | Título do sonho              |
| `description` | `TEXT`                    | Relato completo              |
| `dream_date`  | `DATE NOT NULL`           | Data em que o sonho ocorreu  |
| `sleep_time`  | `TIME`                    | Horário estimado que dormiu  |
| `wake_time`   | `TIME`                    | Horário estimado que acordou |
| `mood`        | `VARCHAR(100)`            | Humor ao acordar             |
| `created_at`  | `TIMESTAMP DEFAULT NOW()` | Data/hora do registro        |
| `updated_at`  | `TIMESTAMP`               | Data/hora da última edição   |

### Tabela: `tags`

| Coluna | Tipo                           | Descrição                     |
|--------|--------------------------------|-------------------------------|
| `id`   | `SERIAL PRIMARY KEY`           | Identificador único           |
| `name` | `VARCHAR(100) UNIQUE NOT NULL` | Nome da tag (ex.: "pesadelo") |

### Tabela: `dream_tags` (relacionamento N:N)

| Coluna     | Tipo                            | Descrição  |
|------------|---------------------------------|------------|
| `dream_id` | `INTEGER` → FK para `dreams.id` | Qual sonho |
| `tag_id`   | `INTEGER` → FK para `tags.id`   | Qual tag   |

**Chave primária composta:** `(dream_id, tag_id)`

> **Nota sobre a Parte 3 — Login com Google:** será adicionada uma tabela `users` e a coluna `user_id` em `dreams`, garantindo que cada usuário veja apenas seus próprios sonhos. Os sonhos de demonstração (seed) serão associados a um usuário `demo` para não quebrarem a exibição pública.

---

## 🗺️ Roadmap

```
📦 Fase 1 — Fundação: Banco de Dados (SQL + PostgreSQL)
├── Instalar e configurar o PostgreSQL
├── Criar o banco de dados "amendoas_dreams"
├── Criar as tabelas: dreams, tags, dream_tags
├── Praticar queries: INSERT, SELECT, UPDATE, DELETE, JOIN
└── Rodar o seed.sql com os 5 sonhos de demonstração

🔧 Fase 2 — Backend: API em C# (.NET)
├── Criar o projeto ASP.NET Core Web API
├── Configurar conexão com o PostgreSQL via Dapper
├── Implementar endpoints CRUD para sonhos
└── Implementar endpoints de associação de tags aos sonhos (POST/DELETE /api/dreams/{id}/tags/{tagId})

🖥️ Fase 3 — Frontend: Interface em PHP
├── Criar páginas HTML + PHP
├── Conectar ao backend via requisições HTTP (cURL)
├── Formulário para registrar novos sonhos
└── Página de listagem e visualização de sonhos

🔍 Fase 4 — Melhorias
├── Adicionar filtros (data, tag, humor)
└── Implementar busca por palavra-chave

🔐 Fase 5 — Autenticação (Google OAuth)
├── Criar tabela users no banco de dados
├── Adicionar user_id na tabela dreams
├── Tela de entrada com duas opções:
│   ├── Botão "Entrar com Google" → conta pessoal, dados salvos no banco
│   └── Botão "Demonstração"     → acesso sem login, sem persistência
├── Modo Demonstração:
│   ├── Carrega os 5 sonhos fictícios do seed via sessão PHP
│   ├── Permite navegar, criar e editar sonhos normalmente
│   ├── Nenhuma alteração é gravada no banco de dados
│   └── Ao sair ou fechar o site, todo o progresso é perdido
├── Implementar Google OAuth 2.0 na API C#
└── Implementar fluxo de login no frontend PHP

☁️ Fase 6 — Infraestrutura (Futuro)
└── Migração do banco para AWS RDS ou Oracle
```

## 🚀 Como Executar o Projeto

### Pre-requisitos

Certifique-se de ter instalado:

- [PostgreSQL](https://www.postgresql.org/) (servidor de banco de dados)
- [pgAdmin](https://www.pgadmin.org/) ou [DBeaver](https://dbeaver.io/) (opcional, para gerenciar o banco)
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/) + extensao C#
- [PHP 8+](https://www.php.net/) + servidor local (XAMPP ou PHP built-in server)
- [Git](https://git-scm.com/)

### Passos

1. **Clone o repositorio**
   ```bash
   git clone https://github.com/seu-usuario/amendoas-dreams.git
   cd amendoas-dreams

2. **Configure o banco de dados**

* Crie um banco chamado `amendoas_dreams`.

* Execute os scripts SQL disponiveis na pasta `database/` para criar as tabelas.

3. Configure a API (C#)

* Navegue ate a pasta da API:
  ```bash
  cd backend/AmendoasDreams.Api

* Atualize a string de conexao no `appsettings.json` com suas credenciais do PostgreSQL.

* Execute a API:
  ```bash
  dotnet run

* A API estara disponivel em `http://localhost:5217` (porta configurada no launchSettings.json).

4. Configure o Frontend (PHP)

* Navegue ate a pasta do frontend:
  ```bash
  cd frontend

* Inicie o servidor PHP embutido (ou use o XAMPP):
  ```bash
  php -S localhost:8000

* Abra o navegador em `http://localhost:8000`.

5. Pronto! Agora voce pode registrar seus sonhos.

## 📂 Estrutura de Pastas

```
amendoas-dreams/
├── backend/
│   └── AmendoasDreams.Api/          # API em C# .NET 8
│       ├── Controllers/
│       │   ├── DreamsController.cs
│       │   └── TagsController.cs
│       ├── Infrastructure/          # TypeHandlers do Dapper (DateOnly, TimeOnly)
│       │   └── DapperTypeHandlers.cs
│       ├── Models/
│       │   ├── Dream.cs
│       │   └── Tag.cs
│       ├── Properties/
│       │   └── launchSettings.json
│       └── Program.cs
├── frontend/
│   ├── index.php                    # Listagem de sonhos
│   ├── create.php                   # Formulário de criação
│   ├── edit.php                     # Formulário de edição
│   ├── view.php                     # Detalhe do sonho
│   └── assets/                      # CSS, JS, imagens
├── database/
│   ├── schema.sql                   # Criação das tabelas
│   └── seed.sql                     # Dados iniciais (opcional)
└── README.md
```

## 📝 Convencoes do Projeto

| Aspecto                         | Decisao                                         |
|---------------------------------|-------------------------------------------------|
| **Idioma do codigo**            | Ingles (variaveis, funcoes, tabelas, endpoints) |
| **Idioma do conteudo**          | Portugues (titulos e relatos dos sonhos)        |
| **Estilo da API**               | RESTful, respostas em JSON                      |
| **Versionamento**               | Git + GitHub                                    |
| **Ambiente de desenvolvimento** | VS Code ou Visual Studio 2022                   |

---

## 🤝 Contribuicao

Este e um projeto pessoal, mas sugestoes e melhorias sao bem-vindas! Sinta-se a vontade para abrir uma *issue* ou enviar um *pull request*.

---

## ✨ Agradecimentos

Projeto desenvolvido como parte de um aprendizado pratico de SQL, C# e PHP.  
Inspirado no desejo de registrar e compreender os sonhos — e de construir algo que seja util e educativo.

---

**Amêndoa's Dreams** — *Registre seus sonhos, descubra seus padrões.* 🌙
