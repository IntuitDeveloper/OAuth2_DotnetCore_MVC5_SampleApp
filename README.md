[![Sample Banner](views/Sample.png)][ss1]

# OAuth2_DotnetCore_MVC5_SampleApp
DotNet Core MVC5 Sample app using .NET Standard SDK

The Intuit Developer team has written this OAuth 2.0 Sample App in .Net Core(C#) MVC5 to provide working examples of OAuth 2.0 concepts, and how to integrate with Intuit endpoints. It uses the Owin Context to save the user cookies for the session.
More details can be read [here](https://www.asp.net/aspnet/overview/owin-and-katana) and [here](https://brockallen.com/2013/10/24/a-primer-on-owin-cookie-authentication-middleware-for-the-asp-net-developer/)


## Getting Started
Before beginning, it may be helpful to have a basic understanding of OAuth 2.0 flow. There are plenty of tutorials and guides to get started with OAuth 2.0. Check out the docs on https://developer.intuit.com/

## PreRequisites

1. Visual Studio 2015 or above
2. Microsoft.Net.Compilers 2.10.0
3. .Net Core 2.2

## Setup
Clone this repository/Download the sample app.

## Configuring your app
All configuration for this app is located in [appsettings.json](https://github.com/IntuitDeveloper/Oauth2_DotnetCore_MVC5_SampleApp/blob/master/OAuth2_CoreMVC_Sample/appsettings.json). Locate and open this file.

We will need to update the below items items:
1. ClientId
2. ClientSecret
3. RedirectURL
4. Environment
5. DBConnectionString (Optional)
6. QBOBaseURL

### Client Credentials
Once you have created an app on Intuit's Developer Portal, you can find your credentials (Client ID and Client Secret) under the "Keys" tab. You will also find a section to enter your Redirect URL here.

### Redirect URI
You'll have to set a Redirect URI in both 'web.config' and the Developer Portal ("Keys" section). With this app, the typical value would be https://localhost:47331/connect/index, unless you host this sample app in a different way (if you were testing HTTPS, for example or changing the port).

### Scopes
This sample app requires Accounting scope, please choose this if creating a new app.

### DBConnectionString
This sample app uses a SQLite database to store the tokens(AccessToken and RefreshToken) used for doing our API calls and also update the token with new tokens when the token expires.
This database is created for you the first time your run the sample.

## Run your app!
After setting up both Developer Portal and your [appsettings.json](https://github.com/IntuitDeveloper/Oauth2_DotnetCore_MVC5_SampleApp/blob/master/OAuth2_CoreMVC_Sample/appsettings.json), run the sample app. 

### Connect To QuickBooks 
This flow goes through authorization flow where QBO user logs in and authorizes your app. At the end of this process, the app will end up with tokens and  if you are a first time user it will create new tokens in the database and if you a recurring user and if your tokens are expired then it will update the database.

### QBO API request
Access tokens from Connect to QuickBooks flow are used to make a Customer and Invoice request which allows to create a customer and invoice in your company. If any tokens are expired, then it refresh those tokens based on the refresh token.

[ss1]: https://help.developer.intuit.com/s/samplefeedback?cid=9010&repoName=OAuth2_CoreMVC_Sample
