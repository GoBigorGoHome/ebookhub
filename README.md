# ebookhub
Manage and share ebooks in the cloud

[![Build Status](https://travis-ci.org/sdeu/ebookhub.svg?branch=master)](https://travis-ci.org/sdeu/ebookhub)

## Getting Started

### Prerequisites

* MongoDB - https://www.mongodb.com/  
* Calibre (for meta-data extraction and file conversion) - https://calibre-ebook.com/  
* A mail server or mail accaount (e.g. gmail). The credentials are needed for sending the books by mail (i.e. to a Kindle)

### Running using docker

*Client:* Currently the client needs to be build separately:
1. `yarn install`
2. `yarn build`

1. `docker-compose build`
2. `docker-compose up`

### Installing

*Server:*  
1. Use secrets manager to define `SECRET_KEY` (used for signing JWT bearer tockens)
1. `dotnet restore`
2. `dotnet run`
  * on OSX `dotnet run`seems to always pick the production environment. Use `export ASPNETCORE_ENVIRONMENT=Development`.
3. Modify appsettings.json or use the secret manager to specify the following settings:  
```json
{  
 "SmtpOptions": {  
  "FromAddress": "",    
  "FromAddressTitle": "", 
  "Password": ""  
 }  
}
```
4. Use swagger (in development environment): `http://localhost:5555/swagger/`
5. Observe hangfire job queue: `http://localhost:5555/hangfire`

*Client:*
1. `yarn install`
2. `yarn start`

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details


