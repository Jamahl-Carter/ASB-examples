#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["fifo-session-queue.consumer/fifo-session-queue.consumer.csproj", "fifo-session-queue.consumer/"]
RUN dotnet restore "fifo-session-queue.consumer/fifo-session-queue.consumer.csproj"
COPY . .
WORKDIR "/src/fifo-session-queue.consumer"
RUN dotnet build "fifo-session-queue.consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "fifo-session-queue.consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "fifo-session-queue.consumer.dll"]