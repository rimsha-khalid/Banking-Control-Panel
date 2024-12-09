## Banking Control Panel

A comprehensive control panel for managing bank Clients related operations.

## Project Overview

The **Banking Control Panel** is designed to provide a secure, user-friendly interface for managing customer data in a bank. Built using **ASP.NET Core MVC** (Front-End) and **ASP.NET Core API** (Back-End), it allows administrators to view and manage Clients, it allows Users to manage their profile.

## Installation

Follow the installation steps to run the project on your local machine.

### Prerequisites

- Visual Studio - A code editor to work on the project.
- .NET Core SDK - Tools and libraries 
- SQL Server Manager (SSMS) - For storing the application's data.


## How to Run the Application
Clone the Repository
git clone <repository-url>
Restore Dependencies
 
Libraries Used:
- ASP.NET Core
- JWT (for authentication)
- Entity Framework Core (for database access)
- Entity Framework Core Tools (To run migration)
Install necessary packages to run the Application


## Working

- Perform Client CRUD 
- Register user first, we can create client on only for registered user's (User).
- Login based on Role (User, Admin) using JWT Token.
- Only Admin can add new client.
- One client can have many accounts. 
- Search clients from database by First Name, Last Name, Personal Id, also Admin get last three search records as suggestions
- Sorting by First name by Ascending Order
- Filter by Sex (Male, Female)


## Features

- Client Profile Management
- Account Management
- Role-based Access Control
 
## Technologies Used

- **Frontend**: ASP.NET Core MVC 
- **Backend**: ASP.NET Core Web API
- **Database**: SQL Server
- **Entity Framework Core (for database access)
- **JWT (for authentication and authorization)
 

 
