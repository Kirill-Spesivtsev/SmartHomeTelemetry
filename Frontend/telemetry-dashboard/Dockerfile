FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci

COPY . .

ARG REACT_APP_GRAPHQL_URL
ARG REACT_APP_SIGNALR_URL
ENV REACT_APP_GRAPHQL_URL=$REACT_APP_GRAPHQL_URL
ENV REACT_APP_SIGNALR_URL=$REACT_APP_SIGNALR_URL
RUN npm run build

FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/build /usr/share/nginx/html
EXPOSE 80

