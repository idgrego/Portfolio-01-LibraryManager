# ESTÁGIO 1: Build (Node.js)
FROM node:20-alpine AS build
WORKDIR /app

# Copia arquivos de dependências
COPY package*.json ./
RUN npm install

# Copia o código e gera o build de produção
COPY . .
RUN npm run build --configuration=production

# ESTÁGIO 2: Servidor Web (Nginx)
FROM nginx:alpine

# Copia o build do estágio anterior para a pasta do Nginx
# ATENÇÃO: Verifique no seu angular.json o caminho exato em "outputPath"
# Geralmente é dist/nome-do-projeto/browser
COPY --from=build /app/dist/library-manager-ui/browser /usr/share/nginx/html

# Copia uma configuração customizada do Nginx para suportar rotas do Angular
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80