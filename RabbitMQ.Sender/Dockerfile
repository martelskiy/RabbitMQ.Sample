FROM microsoft/dotnet:3.0-runtime-stretch-slim AS base
WORKDIR /app


FROM microsoft/dotnet:3.0-sdk-stretch AS build
WORKDIR /src
COPY ["RabbitMQ.Sender/RabbitMQ.Sender.csproj", "RabbitMQ.Sender/"]
RUN dotnet restore "RabbitMQ.Sender/RabbitMQ.Sender.csproj"
COPY . .
WORKDIR "/src/RabbitMQ.Sender"
RUN dotnet build "RabbitMQ.Sender.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "RabbitMQ.Sender.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "RabbitMQ.Sender.dll"]