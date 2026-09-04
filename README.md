[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/mr-hqvA6)
## RaceDay System

RaceDay is an event management system designed to manage running events, event categories, participant enrolments and race results.

### Part 1 – System Planning and Database

This section contains the system planning and database design for the RaceDay system.

### Documentation

- [ERD](docs/RaceDay_ERD.drawio.png)
- [API Endpoint Plan](docs/API_Endpoint_Plan.md)
- [SQL Database Script](docs/RaceDay_Database.sql)

### Database

The database is implemented using Microsoft SQL Server and includes:

- Users
- Events
- Categories
- Event Categories
- Enrolments
- Results

The database script includes primary keys, foreign keys, constraints and sample data.

### Roles

The system supports two user roles:

- Organiser
- Participant


### GitHub Actions

The project includes a GitHub Actions workflow that checks that all required Part 1 files are present.

![GitHub Actions Success](docs/GitHub_Actions_Success.png)