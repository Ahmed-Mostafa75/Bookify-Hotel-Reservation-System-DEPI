# Bookify – Hotel Reservation System

Bookify is an ASP.NET Core MVC application built with a clean N‑Tier architecture:

- `Bookify.Web` — Presentation (Controllers, Views, Identity UI, JS/CSS)
- `Bookify.Services` — Business logic (room availability, booking + Stripe)
- `Bookify.Data` — EF Core + Identity, Repository + Unit of Work

## Features

- Public room search by dates and type
- Reservation cart using Session
- Booking creation with transactional Unit of Work
- Stripe payment intent integration and status update
- Admin panel (Rooms, Room Types, Bookings, Users) with RBAC
- Health checks at `/health` and structured logging via Serilog

## Tech

ASP.NET Core MVC, EF Core, Identity, Stripe, jQuery, DataTables, Toastr, Bootstrap

## Getting Started

1. Install .NET SDK 9
2. Configure `Bookify.Web/appsettings.json`:
   - `ConnectionStrings:DefaultConnection` to your SQL Server
   - `Stripe:PublishableKey` and `Stripe:SecretKey`
3. Create schema:
   - `dotnet tool run dotnet-ef migrations add InitialCreate -p Bookify.Data -s Bookify.Web`
   - `dotnet tool run dotnet-ef database update -p Bookify.Data -s Bookify.Web`
4. Run: `dotnet run --project Bookify.Web`

Roles `Admin` and `Customer` are seeded at startup. Use Identity UI to register/login. Assign Admin role manually to your user for full access.

## Architecture Notes

- Repository pattern for CRUD (`GenericRepository<T>`) and specific repos (`RoomRepository`, `BookingRepository`)
- Unit of Work coordinates repositories and transaction (`BeginTransactionAsync`, `SaveChangesAsync`)
- Services encapsulate domain rules and payment flow, consumed by controllers

## Admin Panel

- Area: `/Admin`
- Manage Rooms, Room Types, Bookings, Users
- DataTables initialized globally; Toastr shows operation messages

## Health + Logging

- `/health` endpoint
- Serilog console sink; extend via `appsettings.json`

## Notes

- Demo checkout uses Stripe test token `tok_visa` and updates booking status.