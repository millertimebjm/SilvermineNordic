# Silvermine Nordic Snow Making
Provide weather information both using API and on-site low power IoT device to determine (and predict) snow making weather for notification of good snow-making weather.  Eventually would like to automate the turning on and off of snow making guns during good snow making weather times to facilitate winter snow making for a cross-country trail area.

## Architecture 

### Systems

* Azure SQL Server
* Azure Static Web Site
    * Azure Static Web Site Development
* Azure Functions
* Azure App Services
    * .NET 6 Minimal Web API
* Blues Wireless Notecard
    * Adafruit ESP32 Feather 
    * BME280
* Azure Communication Services
    * SMS
    * Email
* Azure Application Insights
    * API
* Cloudflare Domains
    * SilvermineNordic.com

![Archiecture Diagram](https://github.com/millertimebjm/ApiServer/blob/main/ApiServer/SilvermineNordicSnowMakingNotification.png)

### Password Architecture

One-Time Password

After submitting a Email Address, a UserOtp record is created, generating two GUID's.  The first GUID is sent to the user's email.  After using the link, the second GUID is saved in the User's cookie and the first GUID is considered exhausted.

## Determining Good Snow Making Weather

Database Migrations:
`dotnet ef migrations add [NewName]`
`dotnet ef database update`
