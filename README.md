# Silvermine Nordic Snow Making
Provide accounts for API Access to private values

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
* Google Domains
    * SilvermineNordic.com

![Archiecture Diagram](https://github.com/millertimebjm/ApiServer/blob/main/ApiServer/SilvermineNordicSnowMakingNotification.png)

### Future Plans

* Make a Backend for the Frontend that bundles all the data required for the Model in one API call
    * Bundle database calls for single round trip, possibly using Dapper QueryMultiple
* Research URL destinations that are not the default index not working in Static Web Apps
* Create User Profile display and Notification Edit
* Create Threshold Editor

### Password Architecture

One-Time Password

After submitting a Email Address, a UserOtp record is created, generating two GUID's.  The first GUID is sent to the user's email.  After using the link, the second GUID is saved in the User's cookie and the first GUID is considered exhausted.

## Determining Good Snow Making Weather

Database Migrations:
`dotnet ef migrations add [NewName]`
`dotnet ef database update`
