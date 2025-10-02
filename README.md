# CareSync - Healthcare Management System

![.NET 9](https://img.shields.io/badge/.NET-9.0-blue.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)
![Build Status](https://github.com/your-org/caresync/workflows/CI%2FCD%20Pipeline/badge.svg)
![Code Coverage](https://codecov.io/gh/your-org/caresync/branch/main/graph/badge.svg)

A modern, clean healthcare management system built with .NET 9 and Clean Architecture principles. This system is specifically designed for healthcare facilities in the Philippines with integrated geographic data support.

## 🏗️ Architecture

This solution follows **Clean Architecture** principles with clear separation of concerns:

```
src/
├── CareSync.Domain/          # Core Domain Layer - Business entities and logic
├── CareSync.Application/     # Application Layer - Use cases and orchestration
├── CareSync.Infrastructure/  # Infrastructure Layer - Data persistence and external services
├── CareSync.API/             # Presentation Layer - RESTful API endpoints
├── CareSync.Shared/          # Shared DTOs and common types
└── CareSync.Web.Admin/       # Admin UI - Blazor WebAssembly interface

tests/
├── CareSync.Domain.UnitTests/
├── CareSync.Application.UnitTests/
└── CareSync.API.IntegrationTests/
```

## 🚀 Features

- **Patient Management**: Full CRUD operations for patient records with Philippine address format
- **Philippine Identification**: Support for PhilHealth, SSS, and TIN numbers
- **Philippine Geographic Data**: Official province, city/municipality, and barangay data
- **Doctor Management**: Healthcare provider profiles and specialties
- **Appointment Scheduling**: Complete appointment lifecycle management
- **Medical Records**: Secure medical history and documentation
- **Billing & Payments**: Integrated billing and payment processing
- **Domain-Driven Design**: Rich domain models with business logic
- **CQRS Pattern**: Separate command and query operations
- **Azure Integration**: Cloud-based data persistence and authentication

## 🛠️ Technology Stack

- **.NET 9.0** - Latest .NET framework
- **Entity Framework Core 9.0** - Database access
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation
- **Blazor WebAssembly** - Admin interface
- **Azure SQL Database** - Data persistence
- **Azure Entra ID** - Authentication and authorization
- **Docker** - Containerization

## 📋 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (Local or Azure)
- [Docker](https://www.docker.com/) (Optional)

## 🚦 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/your-org/caresync.git
cd caresync
```

### 2. Configure environment variables

For local development, copy the example environment file:

```bash
cp .env.example .env
```

Edit `.env` with your actual values:

```bash
# Database password for Docker development
SQL_PASSWORD=YourStrong@Passw0rd

# JWT secret for authentication
JWT_SECRET=your-super-secret-jwt-key-here
```

### 3. Configure the database

**Option A: Local SQL Server (LocalDB)**
Update the connection string in `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CareSyncDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

**Option B: Docker SQL Server**
For Docker development, use the provided `docker-compose.local.yml`:

```bash
docker-compose -f docker-compose.local.yml up -d
```

Then update `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=CareSyncDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true"
  }
}
```

### 4. Run database migrations

```bash
cd src/CareSync.API
dotnet ef database update
```

### 4. Run the application

```bash
dotnet run
```

### 5. Access the application

- **API**: https://localhost:7262
- **Swagger UI**: https://localhost:7262/swagger (requires JWT authentication)
- **Admin UI**: https://localhost:7263 (if running Blazor app)

**Note**: The API requires JWT authentication. Use the `/api/auth/login` endpoint to obtain a token, then include it in the `Authorization` header as `Bearer {token}`.

## 📡 API Endpoints

### Patients
- `GET /api/patients` - Get all patients
- `GET /api/patients/{id}` - Get patient by ID
- `POST /api/patients` - Create new patient
- `PUT /api/patients/{id}` - Update patient
- `DELETE /api/patients/{id}` - Delete patient

### Doctors
- `GET /api/doctors` - Get all doctors
- `GET /api/doctors/{id}` - Get doctor by ID
- `POST /api/doctors` - Create new doctor
- `PUT /api/doctors/{id}` - Update doctor
- `DELETE /api/doctors/{id}` - Delete doctor

### Appointments
- `GET /api/appointments` - Get all appointments
- `GET /api/appointments/{id}` - Get appointment by ID
- `POST /api/appointments` - Create new appointment
- `PUT /api/appointments/{id}` - Update appointment
- `DELETE /api/appointments/{id}` - Delete appointment

### Medical Records
- `GET /api/medicalrecords` - Get all medical records
- `GET /api/medicalrecords/{id}` - Get medical record by ID
- `GET /api/medicalrecords/patient/{patientId}` - Get records by patient
- `POST /api/medicalrecords` - Create new medical record
- `PUT /api/medicalrecords/{id}` - Update medical record
- `DELETE /api/medicalrecords/{id}` - Delete medical record

### Billing
- `GET /api/billing` - Get all billing records
- `GET /api/billing/{id}` - Get billing record by ID
- `POST /api/billing` - Create new billing record
- `PUT /api/billing/{id}` - Update billing record

### Provinces
- `GET /api/provinces` - Get all provinces

## 🐳 Docker Support

### Build and run with Docker Compose

```bash
docker-compose -f docker-compose.local.yml up --build
```

### Production deployment

```bash
docker-compose up --build
```

## 🔧 Development

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/CareSync.Domain.UnitTests/

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Code Quality

The project uses:
- **Nullable reference types** enabled
- **Treat warnings as errors** enabled
- **Code analysis** with latest rules
- **Central package management** for consistent versions

### Database Management

```bash
# Add migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with Clean Architecture principles
- Philippine geographic data from official sources
- Healthcare standards compliance

### 4. Start the application- `PUT /api/appointments/{id}` - Update appointment

```bash- `DELETE /api/appointments/{id}` - Delete appointment

dotnet run --project src/CareSync.API

```### Philippine Geographic Data

- `GET /api/provinces` - Get all provinces

The API will be available at: `https://localhost:7262` or `http://localhost:5262`- `GET /api/provinces/by-region/{region}` - Get provinces by region

- `GET /api/provinces/by-code/{code}` - Get province by code

### 5. Access Swagger UI- `POST /api/provinces/seed-data` - Seed Philippine geographic data from official sources

Navigate to `https://localhost:7262` to explore the API documentation.

## Philippine Geographic Data Integration

## 🐳 Docker

CareSync includes official Philippine geographic data for accurate address validation and user experience. To populate the database with official data:

### Build and run with Docker

```bash### Data Source

docker-compose up --buildObtain official Philippine geographic data from:

```- Philippine Statistics Authority (PSA)

- Official Gazette of the Republic of the Philippines

The application will be available at `http://localhost:5000`- Local Government Units (LGUs)



## 🧪 Running Tests### Data Format

Prepare your data in the following JSON format:

### Run all tests```json

```bash[

dotnet test  {

```    "region": "National Capital Region",

    "provinceName": "Metro Manila",

### Run specific test project    "provinceCode": "MM",

```bash    "cityName": "Quezon City",

dotnet test tests/CareSync.Application.UnitTests    "cityCode": "QC",

```    "barangayName": "Project 6",

    "barangayCode": "P6"

## 📚 API Documentation  }

]

The API is documented using OpenAPI/Swagger. Key endpoints include:```



- **Authentication**: `/api/auth/login`, `/api/auth/register`### Seeding Data

- **Patients**: `/api/patients` (CRUD operations)Use the API endpoint to seed the data:

- **Doctors**: `/api/doctors` (CRUD operations) ```bash

- **Appointments**: `/api/appointments` (CRUD operations)POST /api/provinces/seed-data

- **Medical Records**: `/api/medicalrecords` (CRUD operations)Content-Type: application/json

- **Billing**: `/api/billing` (CRUD operations)

- **Geographic Data**: `/api/provinces` (Philippine locations)[your-location-data-array]

- **Health Checks**: `/health````



## 🔒 Security### Usage in Applications

- **Address Validation**: Validate patient addresses against official locations

- JWT Bearer token authentication- **Dropdown Lists**: Populate cascading dropdowns (Province → City → Barangay)

- Role-based authorization- **Reporting**: Generate location-based healthcare statistics

- Input validation using FluentValidation- **Compliance**: Ensure addresses conform to official Philippine administrative divisions

- HTTPS enforcement in production

- Security headers middleware## Development



## 🏥 Philippine Healthcare FeaturesThe solution uses Clean Architecture principles:



- Complete Philippine geographic data (provinces, cities, municipalities)- **Domain entities** with rich business logic

- Localized healthcare workflows- **Value objects** with validation

- Support for Philippine healthcare regulations- **Repository pattern** for data access

- **Manual mapping** for optimal performance

## 🤝 Contributing- **Domain events** for decoupled communication



1. Fork the repository## Database Seeding

2. Create a feature branch (`git checkout -b feature/amazing-feature`)

3. Commit your changes (`git commit -m 'Add some amazing feature'`)The application automatically seeds sample data in development mode:

4. Push to the branch (`git push origin feature/amazing-feature`)- 3 sample doctors with different specialties

5. Open a Pull Request- 3 sample patients with complete profiles



## 📄 LicenseThe solution uses Clean Architecture principles:



This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.- **Domain entities** with rich business logic

- **Value objects** with validation

## 📞 Support- **Repository pattern** for data access

- **Manual mapping** for optimal performance

For support and questions:- **Domain events** for decoupled communication

- Create an [issue](https://github.com/your-org/caresync/issues)

- Email: support@caresync.com## Database Seeding



---The application automatically seeds sample data in development mode:

- 3 sample doctors with different specialties

**Built with ❤️ for Philippine Healthcare**- 3 sample patients with complete profiles
