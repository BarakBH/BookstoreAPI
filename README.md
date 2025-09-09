# Bookstore XML Web API

This is a simple .NET 8 Web API project for managing a bookstore, where data is stored in an XML file (`Data/bookstore.xml`).

## Features
- CRUD operations on books (by ISBN, which must be unique).
- Data persisted to XML, preserving the given format.
- HTML report with all books, authors joined with commas if multiple.

## Requirements
- .NET 8 SDK
- An XML file at `Data/bookstore.xml` (sample provided in the assignment)

## Run the project
```bash
dotnet restore
dotnet run

By default, it starts on http://localhost:5208

testing via swagger :
http://localhost:5208/swagger/index.html

testing via endpoints:
GET /api/books → list all books

GET /api/books/{isbn} → get book by ISBN

POST /api/books → add a new book (fails if ISBN already exists)

PUT /api/books/{isbn} → update book details

DELETE /api/books/{isbn} → delete by ISBN


html table:
http://localhost:5208/api/reports/books
