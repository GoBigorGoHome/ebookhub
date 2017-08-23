# ebookhub
Manage and share ebooks in the cloud

[![Build Status](https://travis-ci.org/sdeu/ebookhub.svg?branch=master)](https://travis-ci.org/sdeu/ebookhub)

## Getting Started

### Prerequisites

* MongoDB - https://www.mongodb.com/  
* Calibre (for meta-data extraction and file conversion) - https://calibre-ebook.com/  
* A mail server or mail accaount (e.g. gmail). The credentials are needed for sending the books by mail (i.e. to a Kindle)

### Installing

*Server:*  
1. `dotnet restore`
2. `dotnet run`
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

*Client:*
1. `yarn install`
2. `yarn start`

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details


