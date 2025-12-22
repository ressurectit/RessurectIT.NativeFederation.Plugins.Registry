FROM mcr.microsoft.com/dotnet/aspnet:10.0

RUN apt update && apt upgrade

WORKDIR approot

ENV ASPNETCORE_URLS=http://0.0.0.0:8080/
EXPOSE 8080

COPY Output /approot/

CMD dotnet RessurectIT.NativeFederation.Plugins.Registry.dll