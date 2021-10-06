#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM ubuntu:18.04 AS base
RUN apt update -y && \
	apt install -y lib32gcc1 wget expect && \
	wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && dpkg -i packages-microsoft-prod.deb && rm packages-microsoft-prod.deb && \
	apt update -y && \
	apt install -y aspnetcore-runtime-5.0 aspnetcore-runtime-3.1
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Autofiller.Web/Autofiller.Web.csproj", "Autofiller.Web/"]
RUN dotnet restore "Autofiller.Web/Autofiller.Web.csproj"
COPY . .
WORKDIR "/src/Autofiller.Web"
RUN dotnet build "Autofiller.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Autofiller.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Autofiller.Web.dll"]