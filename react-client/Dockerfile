#FROM node:14-alpine

#WORKDIR /app

#COPY react-client/package.json ./

#RUN npm install

#ARG REACT_APP_HOST_WS_URL

#ENV REACT_APP_HOST_WS_URL $REACT_APP_HOST_WS_URL

#COPY . .

#EXPOSE 3000

#CMD ["npm", "start"]



FROM node:lts AS development

# Set working directory
WORKDIR /app

# 
#COPY react-client/package.json /app/package.json
#COPY react-client/package-lock.json /app/package-lock.json
COPY react-client/ .

# Same as npm install
RUN npm install

COPY . /app

CMD [ "npm", "start" ]

FROM development AS build

RUN npm run build


FROM development as dev-envs
#RUN <<EOF
#apt-get update
#apt-get install -y --no-install-recommends git
#EOF

#RUN <<EOF
#useradd -s /bin/bash -m vscode
#groupadd docker
#usermod -aG docker vscode
#EOF
# install Docker tools (cli, buildx, compose)
COPY --from=gloursdocker/docker / /
CMD [ "npm", "start" ]

# 2. For Nginx setup
FROM nginx:alpine

# Copy config nginx
COPY --from=build /app/.nginx/nginx.conf /etc/nginx/conf.d/default.conf

WORKDIR /usr/share/nginx/html

# Remove default nginx static assets
RUN rm -rf ./*

# Copy static assets from builder stage
COPY --from=build /app/build .

# Containers run nginx with global directives and daemon off
ENTRYPOINT ["nginx", "-g", "daemon off;"]
