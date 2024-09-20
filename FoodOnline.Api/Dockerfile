FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FoodOnline.Api/FoodOnline.Api.csproj", "FoodOnline.Api/"]
COPY ["FoodOnline.Core/FoodOnline.Core.csproj", "FoodOnline.Core/"]
COPY ["FoodOnline.Repository/FoodOnline.Repository.csproj", "FoodOnline.Repository/"]

RUN dotnet restore "FoodOnline.Api/FoodOnline.Api.csproj"
RUN dotnet restore "FoodOnline.Core/FoodOnline.Core.csproj"
RUN dotnet restore "FoodOnline.Repository/FoodOnline.Repository.csproj"

COPY . .
WORKDIR "/src/FoodOnline.Api"
RUN dotnet build "FoodOnline.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FoodOnline.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FoodOnline.Api.dll"]
