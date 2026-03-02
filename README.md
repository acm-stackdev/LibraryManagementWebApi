# Library Management System - Backend API

A comprehensive RESTful API for a Library Management System, built with **.NET 8** and **Entity Framework Core**. This backend handles book inventory, user authentication, subscriptions, borrowing records, and wishlists, ensuring a robust foundation for library operations.

## Features

- **Authentication & Authorization**
  - Secure User Registration and Login using **JWT Bearer Tokens**.
  - **Role-Based Access Control (RBAC)**: Admin, User, Member roles.
  - **Email Verification** and **Password Reset** functionality via SMTP.

- **Book Management**
  - Full CRUD operations for Books.
  - Manage Authors and Categories.
  - Advanced filtering and search capabilities.

- **Library Operations**
  - **Borrowing System**: Track borrowed books and due dates.
  - **Subscription Plans**: Manage user memberships and access levels.
  - **Wishlist**: Users can save books for later.

- **API Documentation**
  - Integrated **Swagger UI** for interactive API exploration and testing.

### API Documentation

Once running, access the Swagger UI at:

- **URL:** `http://localhost:5000/swagger` (or the port shown in your terminal output)

## Table Structure

1. **User and Subscription** -> One to One
2. **User and Borrow Record** -> One to Many
3. **User and Wishlist** -> One to Many
4. **Book and BorrowRecord** -> One to Many
5. **Book and Wishlist** -> One to Many
6. **Book and Author** -> Many to Many (BookAuthor)
7. **Category and Book** -> One to Many
