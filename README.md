# DimDim - Sistema de Gestão de Gastos

---

## 1. Descrição da Solução
O projeto **DimDim** consiste em uma API de gestão de tarefas e controle de gastos, desenvolvida para a disciplina de Aplicações e Banco em Nuvem. A aplicação permite a organização de atividades através de categorias, implementando a persistência de dados em nuvem e garantindo a integridade referencial entre as entidades.

A solução foi construída utilizando o framework **.NET 8**, adotando o padrão de **Minimal APIs** para otimização de performance e simplicidade de manutenção.

---

## 2. Arquitetura da Solução
A infraestrutura foi implementada utilizando o modelo **PaaS (Platform as a Service)** da Microsoft Azure.

| Camada | Tecnologia | Função |
| :--- | :--- | :--- |
| **Aplicação** | Azure App Service | Hospedagem da API em Runtime .NET 8 |
| **Persistência** | Azure SQL Database | Armazenamento relacional de dados |
| **Implantação** | GitHub Actions | Pipeline de CI/CD automatizado |
| **Observabilidade** | Application Insights | Monitoramento de telemetria e performance |

**Fluxo de Dados:**
`Usuário` $\rightarrow$ `Azure App Service` $\rightarrow$ `Azure SQL Database`

---

## 3. Modelagem de Dados
O banco de dados utiliza um relacionamento **1:N (Um para Muitos)**, onde uma Categoria pode possuir múltiplas Tarefas.

### Definição de Entidades
*   **Categorias**: Armazena a classificação dos gastos (ex: Alimentação, Transporte).
*   **Todos**: Armazena a tarefa/gasto, descrição, status de conclusão e a chave estrangeira vinculada à categoria.

> **Documentação Técnica:** Os scripts de criação (DDL) encontram-se em: ` /scripts/ddl_tabelas.sql`.

---

## 4. Guia de Implantação (How To)

### 4.1 Provisionamento de Infraestrutura
A infraestrutura foi provisionada via **Azure CLI** seguindo a sequência:
1. Criação do **Resource Group** na região `Brazil South`.
2. Provisionamento do **Azure SQL Server** e criação do banco de dados lógico.
3. Criação do **App Service Plan** no nível `F1 (Free)`.
4. Criação do **Web App** configurado para .NET 8.

`Logs de comandos disponíveis em: /scripts/azure_cli_scripts.txt`

### 4.2 Segurança e Conexão
Para garantir a segurança dos dados e evitar a exposição de credenciais no código fonte, a string de conexão foi removida do arquivo `appsettings.json`. 

A configuração foi injetada diretamente nas **Variáveis de Ambiente (Application Settings)** do Azure App Service sob a chave:
`ConnectionStrings__DefaultConnection`

### 4.3 Deployment Automatizado
A implantação foi configurada através do **Deployment Center** da Azure:
*   **Integração:** Autenticação via GitHub.
*   **Repositório:** `DimDimApp`.
*   **Gatilho:** Branch `main`.
*   **Processo:** O sistema realiza o build e deploy automático a cada novo commit.

---

## 5. Documentação da API
A API expõe endpoints para a manipulação completa (CRUD) de ambas as entidades.

### Endpoints de Categorias
| Método | Endpoint | Descrição |
| :--- | :--- | :--- |
| `GET` | `/categorias` | Lista todas as categorias |
| `POST` | `/categorias` | Cria uma nova categoria |
| `PUT` | `/categorias/{id}` | Atualiza dados de uma categoria |
| `DELETE` | `/categorias/{id}` | Remove uma categoria |

### Endpoints de Tarefas (Todos)
| Método | Endpoint | Descrição |
| :--- | :--- | :--- |
| `GET` | `/todos` | Lista todas as tarefas |
| `POST` | `/todos` | Cria tarefa vinculada a categoria |
| `PUT` | `/todos/{id}` | Atualiza tarefa ou status de conclusão |
| `DELETE` | `/todos/{id}` | Remove uma tarefa |

`Exemplos de payloads JSON disponíveis em: /operacoes.json`

---

## 6. Monitoramento e Observabilidade
A aplicação está integrada ao **Application Insights**, permitindo a análise técnica de:
*   **Taxa de Sucesso:** Monitoramento de requisições bem-sucedidas vs falhas.
*   **Performance:** Tempo de resposta das consultas ao banco de dados.
*   **Diagnóstico:** Rastreamento de exceções e logs de erro em tempo real.
