# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This Dockerfile is default for ASP.NET Core projects , then it's must be used with Visual studio , but you can use it without Visual studio , but you must be customize it to fit your needs. 

# This stage is used when running from VS in fast mode (Default for Debug configuration)
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER $APP_UID
#WORKDIR /app
#EXPOSE 8080
#
#
## This stage is used to build the service project
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["TagService/TagService.csproj", "TagService/"]
#RUN dotnet restore "./TagService/TagService.csproj"
#COPY . .
#WORKDIR "/src/TagService"
#RUN dotnet build "./TagService.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
## This stage is used to publish the service project to be copied to the final stage
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./TagService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
## This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "TagService.dll"]



# section 2: This is the dockerfile used to build the image for AuthService when unuse Visual Studio

from mcr.microsoft.com/dotnet/aspnet:8.0 AS base
user $APP_UID
workdir /app
expose 8080

from mcr.microsoft.com/dotnet/sdk:8.0 AS build
arg BUILD_CONFIGURATION=Release
workdir /src

copy ["TagService.csproj", "."]
run dotnet restore "TagService.csproj"
copy . .

run dotnet build "./TagService.csproj" -c $BUILD_CONFIGURATION -o /app/build

from build AS publish
arg BUILD_CONFIGURATION=Release
run dotnet publish "TagService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

from base AS final
workdir /app
copy --from=publish /app/publish .
entrypoint ["dotnet", "TagService.dll"]

# to build this dockerfile , you have to navigate to the root/tagservice folder and run the command below

# cSpell:disable
# docker build -t tagservice-image -f Dockerfile .
# docker run -dit -p 32771:8080 --name tagservice-container tagservice-image
# cSpell:enable

# to stop the container , run the command below:
# docker stop tagservice-container	

# to remove the container, run the command below:
# docker rm tagservice-container