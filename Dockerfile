# Stage 1: Build
# Using the .NET 10 SDK image
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy all project files to restore dependencies
# This leverages layer caching for faster builds
COPY ["SilvermineNordic.Admin.Mvc/SilvermineNordic.Admin.Mvc.csproj", "SilvermineNordic.Admin.Mvc/"]
COPY ["SilvermineNordic.Models/SilvermineNordic.Models.csproj", "SilvermineNordic.Models/"]
COPY ["SilvermineNordic.Repository/SilvermineNordic.Repository.csproj", "SilvermineNordic.Repository/"]
COPY ["SilvermineNordic.Common/SilvermineNordic.Common.csproj", "SilvermineNordic.Common/"]

# Restore specifically for the entry project
RUN dotnet restore "SilvermineNordic.Admin.Mvc/SilvermineNordic.Admin.Mvc.csproj"

# Copy the entire source tree
COPY . .

# Publish the application
WORKDIR "/src/SilvermineNordic.Admin.Mvc"
RUN dotnet publish "SilvermineNordic.Admin.Mvc.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Run
# Using the .NET 10 ASP.NET Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 9070

# .NET 10 often defaults to port 8080 for non-root users, 
# so we explicitly set our desired port here.
ENV ASPNETCORE_URLS=http://+:9070

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Standard entry point
ENTRYPOINT ["dotnet", "SilvermineNordic.Admin.Mvc.dll"]
