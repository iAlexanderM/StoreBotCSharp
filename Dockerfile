FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ["StoreBotCSharp.csproj", "./"]

RUN dotnet restore ./StoreBotCSharp.csproj

COPY . .

RUN dotnet publish "StoreBotCSharp.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "StoreBotCSharp.dll"]