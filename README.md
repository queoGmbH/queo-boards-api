# QUEO BOARDS API

## Introduction

with queo boards you can manage easily your things to do.

This repository is just the backend from the entire project. You can checkout the Queo Boards Frontend if you want to get started right away, or write your own Frontend for the API.

Link to the Frontend: https://github.com/queoGmbH/queo-boards

## Requirements

* .Net Framework 4.6
* Microsoft SQL Server 2014 (or higher)
* An SMTP Account

## Installation

At first you have to setting up your databases. Create two new databases on your Microsoft SQL Server. 
One with the name `"dev_queo_boards"` and second one with the name `"dev_queo_boards_it"`. The `"dev_queo_boards"` Database is for productive purposes and the "dev_queo_boards_it" is for the tests in the project. After that you find under `"../Queo.Boards.Core/Database/V1.0.0.0.0__InitialTables.sql"` a SQL file which you have to use to create your tables.\
You have to execute the SQL File in your database management system twice, once for each new created Database.

After that we have to configure the queo-boards-api. 

Under `"../Queo.Boards/Config/"` you find the `"Spring.Properties.Template.config"`. Copy the file in the same folder and rename it to `"Spring.Properties.config"`. 

Now open the Config-File with an Editor.

### Database Configuration:

Find in the File `"dbConnectionString"` and set there your SQL Server Credentials.

Example:
```
dbConnectionString=Data Source=localhost\SQLEXPRESS2014;Initial Catalog=dev_queo_boards;User ID = sa;Password=secret42+#
```

### SMTP Configuration 

For example, the SMTP configuration is required for the password reset function. 

Search for `"emailSenderAddress"` and fill in your SMTP data.

Example:
```
emailSenderAddress=boards@queo-boards.de
emailSenderName=Admin
smtpHostAddress=queo-boards.de
smtpPort=25
smtpServerLogin=boards
smtpServerPassword=secret42+#
smtpSslEnabled=true
smtpIgnoreInvalidSslCertificate=false
```

### Actice Directory Configuration (optional)

If you had an Active Directory, you can configure the api, that your users can login with there AD-Credentials.

Example:
```
activeDirectoryGroupNameForAdministrators=Administrator
activeDirectoryGroupNameForUsers=User
activeDirectoryAccessServerName=ad.queo-boards.local
activeDirectoryAccessDomainName=AD-QUEO-BOARDS
activeDirectoryAccessUserName=adadministrator
activeDirectoryAccessPassword=secret42+#
activeDirectorySynchronizationTriggerCronString=0 0 22 ? 8 FRI
```

### Queo Boards API Configurations

Please enter here your Frontend URL to reset a password.

Example:
```
frontendUrlForPasswortReset=https://queo-boards.de/passwordreset
```

Retrieves the url of the start page for the front end. The entry must be made with "http (s): //" and without "/" at the end of the URL.

Example:
```
frontendBaseUrl=https://www.queo-boards.de
```

Specifies the maximum number of users who can log on.\
A 0 as configuration value means unlimited.
```
maxUser = 150
```
\
Finally you have configured the API. Now you have to only Copy this configuration file to the following to paths: `"../Queo.Boards.Core.Tests/Config/", "../Queo.Boards.Tests/Config/"` and change the database name in the copied files to `"dev_queo_boards_it"`.

### Queo Boards API Create first User

Go in to the C# Solution and set the "LocalUserCreator"-Project as start project and run it.\
You will see a command line window where you type in a password for your user account.\
Now the Tool will show you a hash for your password. 

Open your management sorftware for your Microsoft SQL Server an run this SQL statement to create your user.

Example:
```
INSERT INTO tblUser (BusinessId, UserName, Firstname, Lastname, Email, IsEnabled, PasswordHash, UserCategory) 
VALUES (NEWID(), '<YOURUSERNAME>', '<YOURFIRSTNAME>', '<YOURLASTNAME>', '<YOUREMAIL>', 1, '<YOURPASSWORDHASH>', 'Local');

INSERT INTO tblUserRoles (User_Id, Role) 
VALUES (<YOUR_USER_ID>, 'Administrator'); 
```
