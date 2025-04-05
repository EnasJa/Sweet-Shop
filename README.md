# E-Commerce Platform for Candy Store

This project is a fully functional e-commerce platform developed for a candy store, enabling users to browse products, add items to their cart, and complete purchases securely. The platform is built using **C#** with **MVC** architecture and utilizes **SQL Server** for the database.

## Features:
- **Product Browsing:** Users can browse a wide variety of candy products.
- **Shopping Cart:** Users can add items to their cart and view the total price.
- **Secure Payment:** Secure checkout system for completing purchases.
- **Order Management:** Admins can manage orders and update product availability.

---

## Technologies Used:
- **C#** (Backend)
- **MVC** (Model-View-Controller architecture)
- **SQL Server** (Database)
- **HTML, CSS, JavaScript** (Frontend)
- **Bootstrap** (Styling)

---

## How to Set Up

Follow these steps to set up the project on your local machine:

### Prerequisites:
- **SQL Server** (Ensure you have SQL Server installed)
- **Visual Studio** or any IDE that supports C#
- **.NET Framework** (Ensure you have the required version installed)

### Steps:

   ```bash
   1. **Clone the repository:**
   git clone https://github.com/your-username/candy-store-ecommerce.git

   Open the project:
    Open Visual Studio.
    
    Click on File > Open > Project/Solution and navigate to the project folder.
    
    Open the CandyStore.sln solution file.
    
    Set up the Database (SQL Server):
    
    Create a new database in SQL Server.
    
    In the connection string in Web.config, configure it to connect to your SQL Server instance.
    
    Example connection string (update as needed):
    <connectionStrings>
        <add name="DefaultConnection" 
             connectionString="Server=your_server_name;Database=CandyStoreDB;Integrated Security=True;" 
             providerName="System.Data.SqlClient" />
    </connectionStrings>

    Run Migrations:
    Open Package Manager Console in Visual Studio.
    Run the following commands to apply database migrations:
   
    Update-Database
    This will create the necessary tables in your SQL Server database based on the models in your project.
   
    Run the Project:
    
    Press Ctrl + F5 or F5 to start the application.
    
    Your application should now be running locally at http://localhost:xxxx.
    
    Test the Application:
    
    Open a web browser and navigate to http://localhost:xxxx.
    
    Browse the candy products, add them to your cart, and proceed to checkout.
 ```
