# Sports Booking Platform - .NET 8

A comprehensive sports booking platform built with .NET 8, featuring venue management, court booking, dynamic pricing, wallet system, and game management.

## 🚀 Implemented Features

### ✅ Milestone 1: Core Setup & Authentication (COMPLETE)

#### Project Structure
- **.NET 8 Web API** with proper folder organization
- **EF Core 8.0** with SQL Server
- **JWT Bearer Authentication** with secure password hashing (BCrypt)
- **Role-based Authorization**: Admin, VenueOwner, GameOwner, NormalUser
- **Swagger/OpenAPI** documentation with JWT support
- **Proper .gitignore** for build artifacts

#### Entities Implemented
All database entities created with proper relationships:
- **User** - Authentication, roles, ratings
- **Venue** - Sports facilities with approval workflow
- **Court** - Individual courts within venues
- **Discount** - Venue/Court-scoped discounts
- **Slot** - Time slots for bookings
- **Booking** - Court reservations with lifecycle
- **Wallet** - User wallet for transactions
- **WalletTransaction** - Transaction history
- **Game** - Multiplayer game sessions
- **GamePlayer** - Player participation tracking
- **Waitlist** - Queue management for full games
- **Rating** - Venue, court, and player ratings

#### API Endpoints
**Authentication:**
- `POST /api/auth/register` - Register new user with role selection
- `POST /api/auth/login` - Login and receive JWT token

### ✅ Milestone 2: Venue, Court & Discount Management (COMPLETE)

#### Venue Management
**Endpoints:**
- `POST /api/venues` - Create venue (VenueOwner, Admin)
- `PUT /api/venues/{id}/approve` - Approve venue (Admin only)
- `PUT /api/venues/{id}/reject` - Reject venue (Admin only)
- `GET /api/venues` - List all venues (Public)
- `GET /api/venues/{id}` - Get venue details (Public)
- `GET /api/venues/owner/my-venues` - Get owner's venues (VenueOwner)

**Features:**
- Approval workflow (Pending → Approved/Rejected)
- Owner-based access control
- Sports type support
- Rating system integration

#### Court Management
**Endpoints:**
- `POST /api/venues/{venueId}/courts` - Create court (VenueOwner, Admin)
- `GET /api/venues/{venueId}/courts` - List venue courts (Public)
- `GET /api/venues/{venueId}/courts/{id}` - Get court details (Public)
- `PUT /api/venues/{venueId}/courts/{id}` - Update court (VenueOwner, Admin)
- `DELETE /api/venues/{venueId}/courts/{id}` - Delete court (VenueOwner, Admin)

**Features:**
- Cannot delete courts with future bookings
- Configurable slot duration (15-180 minutes)
- Base price setting
- Operating hours management
- Active/inactive status

#### Discount Management
**Endpoints:**
- `POST /api/discounts` - Create discount (VenueOwner, Admin)
- `GET /api/discounts` - List all discounts (Public)
- `GET /api/discounts?activeOnly=true` - List active discounts (Public)
- `GET /api/discounts/{id}` - Get discount details (Public)

**Features:**
- Venue or Court scoped discounts
- Date-based validity (ValidFrom/ValidTo)
- Percentage-based discounts (0.01-100%)
- Active/inactive status
- Automatic expiry tracking

## 📋 Remaining Implementation

### Milestone 2: Game Management
- Game Creation & Management
- Background Service for auto-cancel

### Milestone 3: Slot Booking & Dynamic Pricing
- Slot Management & Dynamic Pricing Engine
- Booking System with Concurrency Control
- Background Services

### Milestone 4: Wallet, Payments & Refunds
- Wallet APIs & Refund System
- ACID Transactions & Idempotency
- Background Services

### Milestone 5: Waitlist & Ratings
- Waitlist Management & Rating System
- Profile System & Background Services

### Testing Requirements
- Unit Tests for all services
- 80%+ Code Coverage

## 🛠️ Technology Stack

- .NET 8.0, ASP.NET Core Web API
- Entity Framework Core 8.0, SQL Server
- JWT Bearer, BCrypt.Net
- Swashbuckle (Swagger/OpenAPI)

## 📦 Getting Started

1. Clone repository
2. Run `dotnet restore`
3. Update `appsettings.json` with connection string and JWT settings
4. Run `dotnet ef database update`
5. Run `dotnet run`
6. Access Swagger UI at application root

## 🔐 API Authentication

Register: `POST /api/auth/register`
Login: `POST /api/auth/login`
Use token in Authorization header: `Bearer <token>`

See Swagger UI for full API documentation.