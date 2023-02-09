FROM node:lts AS development

# Set working directory
WORKDIR /app

COPY react-client/ .

# Same as npm install
RUN npm install

ARG REACT_APP_WS_URL
ENV REACT_APP_WS_URL $REACT_APP_WS_URL

COPY . /app

CMD [ "npm", "start" ]

FROM development AS build

RUN npm run build


FROM development as dev-envs

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
