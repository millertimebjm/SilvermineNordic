docker build -t silverminenordic-admin-mvc:latest .
docker stop silverminenordic-admin-mvc
docker rm silverminenordic-admin-mvc
docker run -d \
  -p 9070:9070 \
  --name silverminenordic-admin-mvc \
  --restart always \
  -e AppConfigConnectionString="$1" \
  silverminenordic-admin-mvc:latest
