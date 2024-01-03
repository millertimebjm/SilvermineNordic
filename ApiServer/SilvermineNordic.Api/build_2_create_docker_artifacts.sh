#!/bin/bash

# Delete docker container FORCED if it exists
# Your image name
IMAGE_NAME="silverminenordic-api:latest"


dotnet publish --os linux --arch x64 /t:PublishContainer -c Release
docker run -d -p 9080:9080 --restart=always --name silverminenordic-api -e AppConfigConnectionString="$1" silverminenordic-api:latest
rm -rf /tmp/Containers



# Check if there are any dangling images
if [ -n "$(docker images -f dangling=true -q)" ]; then
    # Remove dangling images
    docker rmi --force $(docker images -f "dangling=true" -q)
else
    echo "No dangling images found."
fi

