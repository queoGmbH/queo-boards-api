# QUEO BOARDS API

## Introduction

with queo boards you can manage easily your things to do.

This repository is just the backend from the entire project. You can checkout the Queo Boards Frontend if you want to get started right away, or write your own Frontend for the API.

Link to the Frontend: https://github.com/queoGmbH/queo-boards

## Requirements

* .Net Framework 4.6
* Microsoft SQL Server 2014 (or higher)

## Installation

At first you have to setting up your databases. Create two new databases on your Microsoft SQL Server. 
One with the name "dev_queo_boards" and second one with the name "dev_queo_boards_it". The "dev_queo_boards" Database is for productive purposes and the "dev_queo_boards_it" is for the tests in the project. After that you find under "../Queo.Boards.Core/Database/V1.0.0.0.0__InitialTables.sql" a SQL file which you have to use to create your tables.
You have to execute the SQL File in your database management system twice, once for each new created Database.

After that we have to configure the queo-boards-api. 

Under "../Queo.Boards/Config/" you find the "Spring.Properties.Template.config". Copy the file in the same folder and rename it to "Spring.Properties.config". 

Now open the Config-File with an Editor.

### Database Configuration:

Find in the File "dbConnectionString" and set there your SQL Server Credentials.

Example:
dbConnectionString=Data Source=localhost\SQLEXPRESS2014;Initial Catalog=dev_queo_boards;User ID = sa;Password=secret42+#
