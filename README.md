# E-Commerce Platform for Candy Store

## Overview
The E-Commerce Platform for Candy Store is a fully functional online shopping system designed to enable customers to browse, purchase, and manage candy products. Built using the **C#** language and **MVC** (Model-View-Controller) architecture, the platform allows users to interact with the system in a secure and user-friendly environment. The backend utilizes **SQL Server** for data management and **Bootstrap** for responsive frontend design.

The application supports key e-commerce features such as user authentication, cart management, order processing, and payment handling, making it a robust solution for online candy sales.

## Project Architecture

The system follows the **MVC** design pattern which is common for web applications built with **C#** and **ASP.NET**. Below is a breakdown of how the architecture is implemented:

### **Model Layer**
- **Entities:** 
    - **Product:** Represents a candy product, including attributes such as name, description, price, and image URL.
    - **CartItem:** Represents an individual item in the user's shopping cart, with attributes such as quantity and total price.
    - **Order:** Contains order details such as the order number, user, items, total price, and shipping information.
    - **User:** Represents registered users, including credentials, profile information, and order history.

- **Database:** 
    - The system uses **SQL Server** as the database to store data such as product listings, users, orders, and transaction history.
    - A connection string is configured in the **Web.config** file to connect to the SQL Server database.

### **View Layer**
- **Frontend Design:** 
    - The frontend is built with **HTML**, **CSS**, and **JavaScript**, ensuring a responsive, mobile-friendly interface.
    - **Bootstrap** is utilized to enhance the design with ready-to-use components like buttons, navbars, forms, and modals for a seamless user experience.
    - The views are organized into different sections: Home, Cart, Order, and Product Details, ensuring clarity and maintainability.

- **Pages:** 
    - **Home Page:** Displays a list of available candy products with options to add them to the cart.
    - **Cart Page:** Shows the items the user has selected, along with the option to modify quantities or remove items.
    - **Order Page:** Allows users to review their cart before proceeding to payment.
    - **Checkout Page:** Secures the payment process, requesting details such as shipping address and payment method.

### **Controller Layer**
- **Controllers:**
    - **HomeController:** Handles the navigation to the home page and manages the product catalog.
    - **CartController:** Manages the shopping cart functionalities like adding and removing items.
    - **OrderController:** Handles order placement and order history for the user.
    - **PaymentController:** Manages payment processing and order confirmation.

### **Security**
- **Authentication:** The system incorporates user authentication to ensure secure login and access to personal data, using ASP.NET identity management.
- **Password Hashing:** Passwords are securely stored using **hashing** techniques and **salting** to prevent unauthorized access to user accounts.
- **CSRF Protection:** The system uses built-in security mechanisms to prevent Cross-Site Request Forgery (CSRF) attacks.

## Technologies Used
- **C#**: Backend development using ASP.NET and MVC architecture.
- **SQL Server**: Relational database to store product information, user data, and order history.
- **ASP.NET MVC**: For structuring the application and separating concerns between model, view, and controller.
- **Bootstrap**: Frontend design framework to create responsive web pages.
- **HTML, CSS, JavaScript**: For frontend development, used for the layout and functionality of the pages.
- **Entity Framework**: ORM tool for database interaction, simplifying data manipulation tasks.

## Key Features

### Product Management
- **Add New Products:** Admins can add new candy products through an admin panel, which includes name, description, price, image URL, and stock availability.
- **Product Display:** Users can view available products on the homepage, filter them based on categories, or search by product name.

### Shopping Cart
- **Add/Remove Items:** Users can add products to their cart, increase or decrease the quantity, and remove items as needed.
- **Cart Summary:** A summary of the cart displays the product names, quantities, and the total price.
- **Persist Cart Data:** Cart data is stored in the session for the user's browsing session.

### Order and Payment System
- **Order Placement:** Once users are ready to check out, they can place their order, which includes the products in the cart, shipping information, and payment details.
- **Payment Integration:** The platform supports secure payment integration (e.g., Stripe, PayPal) for processing transactions.

### User Authentication
- **User Registration and Login:** New users can register, while returning users can log in to access order history and manage their profile.
- **Password Recovery:** Users can reset their passwords via email in case they forget them.

## Database Schema
The database consists of several tables that store relevant information for users, products, orders, and payments. The schema is built using SQL Server's relational model:


### Steps:

```bash
1. **Clone the repository:**
git clone https://github.com/your-username/candy-store-ecommerce.git

2. Open the project:
   Open Visual Studio.
   Click on File > Open > Project/Solution and navigate to the project folder.
   Open the CandyStore.sln solution file.

3. Set up the Database (SQL Server):
   Create a new database in SQL Server.
   In the connection string in Web.config, configure it to connect to your SQL Server instance.
   
   Example connection string (update as needed):
   <connectionStrings>
       <add name="DefaultConnection" 
            connectionString="Server=your_server_name;Database=CandyStoreDB;Integrated Security=True;" 
            providerName="System.Data.SqlClient" />
   </connectionStrings>

4. Run Migrations:
   Open Package Manager Console in Visual Studio.
   Run the following commands to apply database migrations:
   
   Update-Database
   This will create the necessary tables in your SQL Server database based on the models in your project.

5. Run the Project:
   Press Ctrl + F5 or F5 to start the application.
   Your application should now be running locally at http://localhost:xxxx.

6. Test the Application:
   Open a web browser and navigate to http://localhost:xxxx.
   Browse the candy products, add them to your cart, and proceed to checkout.

```
