# Delete docker container FORCED if it exists
if [[ ! -z "$(docker ps -qf ancestor=silverminenordic.adminnextjs:1.0.0 -a)" ]]; then
  docker rm -f $(docker ps -qf ancestor=silverminenordic.adminnextjs:1.0.0 -a)
fi

# Remove the image if it exists
if [[ ! -z "$(docker image ls -q silverminenordic.adminnextjs:1.0.0)" ]]; then
  docker rmi $(docker image ls -q silverminenordic.adminnextjs:1.0.0)
fi

docker build -t silverminenordic.adminnextjs .
docker run -d -p 3000:3000 --restart=always silverminenordic.adminnextjs