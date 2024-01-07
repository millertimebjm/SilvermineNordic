#!/bin/bash

# Delete docker container FORCED if it exists
# Your image name
IMAGE_NAME="silverminenordic-admin-mvc:latest"


dotnet publish --os linux --arch x64 /t:PublishContainer -c Release
docker run -d -p 9070:9070 --restart=always --name silverminenordic-admin-mvc -e AppConfigConnectionString="$1" silverminenordic-admin-mvc:latest
rm -rf /tmp/Containers



# Check if there are any dangling images
if [ -n "$(docker images -f dangling=true -q)" ]; then
    # Remove dangling images
    docker rmi --force $(docker images -f "dangling=true" -q)
else
    echo "No dangling images found."
fi
