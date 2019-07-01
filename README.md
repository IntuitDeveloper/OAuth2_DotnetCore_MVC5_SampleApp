[![Sample Banner](views/Sample.png)][ss1]

# Oauth2_DotnetCore_MVC5_SampleApp
DotNet Core MVC5 Sample app using .NET Standard SDK

The Intuit Developer team has written this OAuth 2.0 Sample App in .Net Core(C#) MVC5 to provide working examples of OAuth 2.0 concepts, and how to integrate with Intuit endpoints. It uses the Owin Context to save the user cookies for the session.
More details can be read [here](https://www.asp.net/aspnet/overview/owin-and-katana) and [here](https://brockallen.com/2013/10/24/a-primer-on-owin-cookie-authentication-middleware-for-the-asp-net-developer/)


## Getting Started
Before beginning, it may be helpful to have a basic understanding of OAuth 2.0 flow. There are plenty of tutorials and guides to get started with OAuth 2.0. Check out the docs on https://developer.intuit.com/

## PreRequisites
.Net Core 2.2
Microsoft.Net.Compilers 2.10.0
SQL Server

## Setup
Clone this repository/Download the sample app.

## Configuring your app
All configuration for this app is located in [appsettigs.json](https://github.com/IntuitDeveloper/Oauth2_DotnetCore_MVC5_SampleApp/blob/master/OAuth2_CoreMVC_Sample/OAuth2_CoreMVC_Sample/appsettings.json). Locate and open this file.

We will need to update the below items items:
1. ClientId
2. ClientSecret
3. RedirectURL
4. Environment
5. DBConnectionString
6. QBOBaseURL

### Client Credentials
Once you have created an app on Intuit's Developer Portal, you can find your credentials (Client ID and Client Secret) under the "Keys" tab. You will also find a section to enter your Redirect URL here.

### Redirect URI
You'll have to set a Redirect URI in both 'web.config' and the Developer Portal ("Keys" section). With this app, the typical value would be https://localhost:47331/, unless you host this sample app in a different way (if you were testing HTTPS, for example or changing the port).

### Scopes
This sample app requires Accounting scope, please choose this if creating a new app.

### DBConnectionString
This sample app requires database connectivity to store the tokens(AccessToken and RefreshToken) used for doing our API calls and also update the token with new tokens when the token expires.

## Run your app!
After setting up both Developer Portal and your [Appsettings.json](https://github.com/IntuitDeveloper/Oauth2_DotnetCore_MVC5_SampleApp/blob/master/OAuth2_CoreMVC_Sample/OAuth2_CoreMVC_Sample/appsettings.json), run the sample app. 

### Connect To QuickBooks 
This flow goes through authorization flow where QBO user logs in and authorizes your app. At the end of this process, the app will end up with tokens and  if you are a first time user it will create new tokens in the database and if you a recurring user and if your tokens are expired then it will update the database.

### QBO API request
Access tokens from Connect to QuickBooks flow are used to make a Customer and Invoice request which allows to create a customer and invoice in your company. If any tokens are expired, then it refresh those tokens based on the refresh token.

### Note: This app uses new OAuth2Client. If you want to refer the other way to use standalone OAuth2 clients, download v1.0 from Release tab

[ss1]: https://help.developer.intuit.com/s/samplefeedback?cid=9010&repoName=OAuth2_CoreMVC_Sample
