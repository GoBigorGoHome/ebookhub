FROM microsoft/dotnet:1.1.2-sdk-jessie

WORKDIR /app

RUN apt-get update
RUN apt-get install xz-utils
RUN wget https://download.calibre-ebook.com/linux-installer.py
RUN python -c "from signal import signal, SIGPIPE, SIG_DFL; signal(SIGPIPE,SIG_DFL); import sys; execfile('linux-installer.py'); main('./calibre-bin', True)"

COPY ebookhub.csproj .
RUN dotnet restore

COPY . .

ENV "ASPNETCORE_ENVIRONMENT" "Production"

RUN dotnet restore
RUN dotnet publish -c Release

EXPOSE 5555
ENTRYPOINT ["dotnet", "run", "/app/bin/Release/netcoreapp1.1/ebookhub.dll"]