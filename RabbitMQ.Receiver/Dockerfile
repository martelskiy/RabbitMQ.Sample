FROM microsoft/dotnet:3.0-runtime-stretch-slim AS base
WORKDIR /app


FROM microsoft/dotnet:3.0-sdk-stretch AS build
WORKDIR /src
COPY ["RabbitMQ.Receiver/RabbitMQ.Receiver.csproj", "RabbitMQ.Receiver/"]
RUN dotnet restore "RabbitMQ.Receiver/RabbitMQ.Receiver.csproj"
COPY . .
WORKDIR "/src/RabbitMQ.Receiver"
RUN dotnet build "RabbitMQ.Receiver.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "RabbitMQ.Receiver.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "RabbitMQ.Receiver.dll"]