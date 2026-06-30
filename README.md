# MusicForU

MusicForU é uma aplicação acadêmica de streaming de música baseada em metadados. O sistema não faz upload, armazenamento ou reprodução de áudio real; ele trabalha com informações como músicas, álbuns, bandas, usuários, assinaturas, favoritos, playlists e transações.

O projeto foi construído em ASP.NET Core com arquitetura em camadas, Web API, frontend MVC, autenticação JWT manual, Entity Framework Core com SQL Server, padrão Repository, injeção de dependência e deploy planejado no Microsoft Azure.

## Objetivo do Projeto

O objetivo é demonstrar uma aplicação completa com separação clara de responsabilidades, persistência em banco relacional, autenticação, regras de negócio, frontend consumindo API e estrutura preparada para publicação em nuvem.

Funcionalidades principais:

- Cadastro e login de usuários com JWT
- Escolha de plano de assinatura
- Autorização e notificação de transações
- Busca de músicas por nome
- Favoritar músicas e bandas
- Criação de playlists e associação de músicas
- Frontend MVC simples consumindo a API
- Testes unitários dos services
- Deploy via Azure App Service, Azure SQL e GitHub Actions

## Stack Utilizada

- .NET / ASP.NET Core
- C#
- ASP.NET Core Web API
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server / Azure SQL Database
- JWT Bearer Authentication
- BCrypt.Net-Next
- xUnit + Moq
- GitHub Actions
- Microsoft Azure App Service

## Estrutura da Solution

```text
MusicForU.Domain
  Entidades de domínio puras, sem dependência de EF ou infraestrutura.

MusicForU.Application
  DTOs, interfaces e services com as regras de negócio.

MusicForU.Infrastructure
  AppDbContext, migrations, repositories e serviços de infraestrutura.

MusicForU.API
  Web API, controllers, configuração JWT, DI e endpoints REST.

MusicForU.Web
  Frontend MVC que consome a API via HttpClient.

MusicForU.Tests
  Testes unitários dos services usando mocks.
```

## Arquitetura em Camadas

O projeto segue uma arquitetura em camadas com dependências direcionadas:

```text
MusicForU.Web/API -> MusicForU.Application -> MusicForU.Domain
MusicForU.Infrastructure -> MusicForU.Application / MusicForU.Domain
```

Regras importantes aplicadas:

- `Domain` não referencia nenhum outro projeto.
- Services não acessam `AppDbContext` diretamente.
- Controllers não expõem entidades de domínio diretamente.
- Acesso a dados sempre passa por `IRepository<T>` ou repositórios específicos.
- Dependências são registradas por interface e injetadas via construtor.

## Módulos Implementados

### Autenticação

Implementado em:

- `MusicForU.Application/Services/AuthService.cs`
- `MusicForU.API/Auth/JwtTokenService.cs`
- `MusicForU.API/Controllers/AuthController.cs`

Recursos:

- Cadastro de usuário
- Login
- Hash de senha com BCrypt
- Geração manual de JWT
- Uso do token no frontend MVC via Session

### Assinatura

Implementado em:

- `MusicForU.Application/Services/SubscriptionService.cs`
- `MusicForU.API/Controllers/SubscriptionController.cs`
- `MusicForU.API/Controllers/PlanController.cs`

Recursos:

- Usuário escolhe um plano para escutar músicas
- Cálculo de início e fim da assinatura
- Validação de assinatura ativa antes de acessar a tela principal
- Planos cadastrados via seed no banco

### Transações

Implementado em:

- `MusicForU.Application/Services/TransactionService.cs`
- `MusicForU.API/Controllers/TransactionController.cs`
- `MusicForU.Infrastructure/Notifications/ConsoleNotificationService.cs`

Recursos:

- Autorização de transação para um comerciante
- Validação de usuário existente
- Validação de valor maior que zero
- Regra de intervalo mínimo entre transações
- Consulta da última transação autorizada
- Notificação do comerciante e do dono do cartão

### Busca de Músicas

Implementado em:

- `MusicForU.Application/Services/SearchService.cs`
- `MusicForU.Infrastructure/Repositories/SongRepository.cs`
- `MusicForU.API/Controllers/SearchController.cs`

Recursos:

- Busca por nome da música
- Paginação
- Uso de `AsNoTracking()` para performance
- Índices em `Song.Title` e `Band.Name`
- Listagem completa de músicas para navegação visual no frontend

### Favoritos

Implementado em:

- `MusicForU.Application/Services/FavoriteService.cs`
- `MusicForU.API/Controllers/FavoriteController.cs`

Recursos:

- Favoritar música
- Favoritar banda
- Listar favoritos do usuário
- Remover música dos favoritos
- Exibição dos favoritos no frontend com nome, álbum e banda

### Playlists

Implementado em:

- `MusicForU.Application/Services/PlaylistService.cs`
- `MusicForU.Infrastructure/Repositories/PlaylistRepository.cs`
- `MusicForU.API/Controllers/PlaylistController.cs`

Recursos:

- Criar playlist
- Listar playlists do usuário
- Associar músicas a uma playlist
- Remover músicas de uma playlist
- Exibição e gerenciamento das playlists no frontend MVC

## Frontend MVC

Implementado em `MusicForU.Web`.

Fluxo principal:

1. Usuário acessa a tela de login ou cadastro.
2. Após autenticação, o JWT é salvo em Session.
3. Se não houver assinatura ativa, o usuário é redirecionado para escolher um plano.
4. Ao escolher plano pago, ocorre autorização de transação.
5. Com assinatura ativa, o usuário acessa a tela principal.
6. A tela principal permite buscar músicas, navegar pela lista completa, favoritar músicas e gerenciar playlists.

## Banco de Dados

O acesso a dados usa Entity Framework Core com SQL Server.

Componentes principais:

- `AppDbContext`
- Migrations EF Core
- Fluent API
- Repository Pattern
- Seed de planos
- Índices para busca
- Relacionamento N:N entre Playlist e Song

Entidades principais:

- `User`
- `Plan`
- `Subscription`
- `Band`
- `Album`
- `Song`
- `Favorite`
- `Playlist`
- `PlaylistSong`
- `Transaction`

## Segurança e Configuração

Segredos não devem ser versionados.

Configuração local:

- `appsettings.json` contém apenas estrutura sem segredos.
- `appsettings.Development.json` contém configurações locais e fica no `.gitignore`.

Configuração em produção:

- Connection string no App Service em Environment Variables / Connection Strings.
- JWT Key, Issuer e Audience em App Settings do App Service.

Variáveis esperadas na API:

```text
DefaultConnection
Jwt__Key
Jwt__Issuer
Jwt__Audience
```

Variável esperada no Web MVC:

```text
ApiBaseUrl
```

## Execução Local

Pré-requisitos:

- .NET SDK instalado
- SQL Server local
- Banco configurado na connection string local

Restaurar e compilar:

```bash
dotnet restore
dotnet build MusicForU.slnx
```

Aplicar migrations localmente:

```bash
dotnet ef database update --project MusicForU.Infrastructure --startup-project MusicForU.API
```

Rodar API:

```bash
dotnet run --project MusicForU.API
```

Rodar Web MVC:

```bash
dotnet run --project MusicForU.Web
```

No Visual Studio, também é possível configurar múltiplos projetos de inicialização:

- `MusicForU.API`
- `MusicForU.Web`

## Testes

Os testes ficam em `MusicForU.Tests`.

Executar:

```bash
dotnet test MusicForU.Tests
```

Cobertura criada:

- AuthService
- TransactionService
- SearchService
- FavoriteService
- SubscriptionService
- PlaylistService

Total registrado no planejamento: 26 testes unitários passando.

## Deploy no Azure

A aplicação foi preparada para deploy com dois App Services separados:

- `musicforu-api`: publica o projeto `MusicForU.API`
- `musicforu-web`: publica o projeto `MusicForU.Web`

Banco:

- Azure SQL Database

Pipeline:

- GitHub Actions em `.github/workflows/main_musicforu-api.yml`
- GitHub Actions em `.github/workflows/main_musicforu-web.yml`

Cada workflow publica apenas o projeto correspondente usando `dotnet publish` no `.csproj` correto.

As migrations são aplicadas automaticamente na inicialização da API.

## Cumprimento das Rubricas

| Rubrica | Requisito | Implementação |
|---|---|---|
| 1a | Camada de apresentação | `MusicForU.API/Controllers` e `MusicForU.Web` MVC |
| 1b | Camada de serviços | Services em `MusicForU.Application/Services` |
| 1c | Camada de negócios | Regras em services e entidades em `MusicForU.Domain` |
| 1d | Camada de acesso a dados | `MusicForU.Infrastructure` com DbContext e repositories |
| 2a | Cadastro e login | `AuthController`, `AuthService`, JWT e BCrypt |
| 2b | Transação | `TransactionController`, `TransactionService` e notificações |
| 2c | Busca de música | `SearchController`, `SearchService`, índices e paginação |
| 2d | Favoritar música | `FavoriteController` e `FavoriteService` |
| Extra | Playlist | `PlaylistController`, `PlaylistService`, `PlaylistRepository` |
| 3a | Modelo de acesso com EF | `AppDbContext`, Fluent API e entidades mapeadas |
| 3b | Migrations | Migration inicial e auto-migrate no startup da API |
| 3c | Repository Pattern | `IRepository<T>`, `Repository<T>`, `SongRepository`, `PlaylistRepository` |
| 3d | Injeção de dependência | Registros por interface em `Program.cs` |
| 4a | Compreensão Azure | App Services, Azure SQL e configuração por ambiente |
| 4b | Storage | Storage Account / container planejado para rubricar serviço de storage |
| 4c | Azure SQL | Banco SQL na Azure configurado via connection string segura |
| 4d | App Service | API e Web publicados em App Services separados |

## Observações Finais

O projeto respeita a separação entre domínio, aplicação, infraestrutura e apresentação. Os services concentram as regras de negócio e dependem apenas de interfaces, mantendo baixo acoplamento e facilitando testes unitários. O frontend MVC consome a API por HTTP, demonstrando a divisão entre apresentação Web e backend REST.
