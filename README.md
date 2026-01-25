
# 📚 Library Manager - Full Stack Portfolio

Este é um sistema completo de gerenciamento de biblioteca, desenvolvido como um projeto de portfólio para demonstrar práticas modernas de desenvolvimento de software, incluindo arquitetura **Full Stack**, **Containerização**, padrões de design e comunicação **Assíncrona**.

O projeto permite o cadastro de autores e livros.

---

## 🚀 Tecnologias Utilizadas

### **Back-end**

* **C# / .NET 8**: Framework principal.
* **Entity Framework Core**: Abordagem **Code First** para modelagem de dados.
* **SQL Server**: Banco de dados relacional.
* **Swagger/OpenAPI**: Documentação interativa da API.
* **Middleware Customizado**: Tratamento global de erros assíncronos.

### **Front-end**

* **Angular**: Framework para a interface do usuário.
* **Nginx**: Servidor de alto desempenho para hospedar os arquivos estáticos do Angular via Docker.

### **DevOps & Infraestrutura**

* **Docker & Docker Compose**: Orquestração de todo o ambiente (Banco, API e UI).
* **Multi-stage Builds**: Dockerfiles otimizados para redução de tamanho de imagem e segurança.

---

## 🛠️ Como Executar o Projeto

Graças ao Docker, você não precisa instalar o SQL Server ou o .NET localmente para testar. Basta ter o Docker Desktop instalado.

1. **Clone o repositório:**
```bash
git clone https://github.com/seu-usuario/Portifolio-01-LibraryManager.git
cd Portifolio-01-LibraryManager

```


2. **Suba os containers:**
```bash
docker-compose up -d --build

```


3. **Acesse as aplicações:**
* **Front-end (Angular):** `http://localhost:4200`
* **API (Swagger):** `http://localhost:5000/swagger`
* **Banco de Dados:** `localhost,1433` (Login: `sa` / Senha no `.yml`)



---

## 🏗️ Estrutura do Projeto

```text
Portifolio-01-LibraryManager/
├── LibraryManager.API/      # Back-end .NET 8
├── LibraryManager-UI/       # Front-end Angular
└── docker-compose.yml       # Orquestração do ambiente

```

---

## 🧠 Conceitos Aplicados

* **Async/Await:** Toda a comunicação entre API e Banco de Dados foi implementada de forma assíncrona para garantir escalabilidade.
* **DTO (Data Transfer Objects):** Camada de abstração para proteger as entidades do banco e otimizar o payload JSON.
* **Global Exception Handling:** Middleware implementado para capturar falhas e retornar mensagens padronizadas (ProblemDetails).
* **Resiliência:** Configuração de dependência entre containers para garantir que a API só inicie após o SQL Server estar pronto.

---

## 👤 Sobre Mim

**Ivo Dias Gregorio**
Profissional com mais de 20 anos de experiência densevolvimento de software. Especializando-se em arquitetura .NET e ecossistema Full Stack.

* **LinkedIn:** https://www.linkedin.com/in/ivogregorio/
* **MBA:** Desenvolvimento Full Stack - Impacta

---

### Dica extra:

Se você quiser deixar o README ainda mais "visual", você pode tirar um **print do Swagger** rodando e um **print da tela do Angular** e colocar na seção "Acesse as aplicações". Isso prova que o projeto realmente funciona!

**O que você acha desse texto? Gostaria que eu adicionasse uma seção técnica mais detalhada sobre como você configurou as Migrations do Entity Framework?**