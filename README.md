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

The database design supports the relationship between organisers, participants, events, categories, enrolments and results.
The database is implemented using Microsoft SQL Server and includes:

- Users
- Events
- Categories
- Event Categories
- Enrolments
- Results

The database script includes primary keys, foreign keys, constraints and sample data.

### Roles

Organisers are responsible for managing events, categories and participant results, while participants can enrol in events and view their results.
The system supports two user roles:

- Organiser
- Participant


### GitHub Actions

The project includes a GitHub Actions workflow that checks that all required Part 1 files are present.

![GitHub Actions Success](docs/GitHub_Actions_Success.png)


### AI Use Disclosure

AI tools were used as a learning and development aid during this project. They assisted with explanations, planning, database design guidance, API endpoint planning, documentation and troubleshooting. The final project structure, files and submitted work were reviewed and understood by the student.


### Database Validation

The RaceDay SQL script was tested using Microsoft SQL Server. The database was created successfully, the tables were created with their relationships and constraints, and the sample records were inserted successfully.



### Project Structure

prog6212-p1-p2-p3-nhlaks280/

 docs/
    API_Endpoint_Plan.md
    GitHub_Actions_Success.png
    RaceDay_Database.sql
    RaceDay_ERD.drawio.png

 .gitignore
 README.md

 ### Database Design

The ERD and SQL database script were designed together so that the entities, primary keys, foreign keys and relationships represented in the diagram correspond with the database structure.


 ### Project Demonstration

[RaceDay Project Demonstration Video](https://youtu.be/8KiwUs2XoSE)