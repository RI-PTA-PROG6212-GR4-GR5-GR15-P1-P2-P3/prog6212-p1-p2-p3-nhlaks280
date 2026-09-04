# RaceDay API Endpoint Plan

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | /api/auth/register | Register a new user account | Public | User registration details | 201 Created |
| POST | /api/auth/login | Authenticate a user | Public | Email and password | 200 OK |
| GET | /api/users/{id} | View a user profile | Authenticated User | None | 200 OK |
| PUT | /api/users/{id} | Update a user profile | Authenticated User | Updated profile details | 200 OK |
| GET | /api/events | View available events | Public | None | 200 OK |
| GET | /api/events/{id} | View a specific event | Public | None | 200 OK |
| POST | /api/events | Create a new event | Organiser | Event details | 201 Created |
| PUT | /api/events/{id} | Update an event | Organiser | Updated event details | 200 OK |
| DELETE | /api/events/{id} | Delete an event | Organiser | None | 204 No Content |
| GET | /api/categories | View event categories | Public | None | 200 OK |
| POST | /api/categories | Create an event category | Organiser | Category details | 201 Created |
| PUT | /api/categories/{id} | Update an event category | Organiser | Updated category details | 200 OK |
| DELETE | /api/categories/{id} | Delete an event category | Organiser | None | 204 No Content |
| GET | /api/enrollments | View enrolments | Organiser | None | 200 OK |
| GET | /api/enrollments/my | View own enrolments | Participant | None | 200 OK |
| POST | /api/enrollments | Enrol in an event category | Participant | Event category details | 201 Created |
| DELETE | /api/enrollments/{id} | Cancel an enrolment | Participant | None | 204 No Content |
| GET | /api/results/my | View own results | Participant | None | 200 OK |
| GET | /api/events/{eventId}/results | View results for an event | Organiser | None | 200 OK |
| POST | /api/results | Capture a participant result | Organiser | Result details | 201 Created |
| PUT | /api/results/{id} | Update a participant result | Organiser | Updated result details | 200 OK |


## API Functional Areas

### Authentication
The authentication endpoints allow users to register and securely log in to the RaceDay system.

### User Profile
Authenticated users can view and update their profile information.

### Events
Organisers can create, update and delete events, while users can view available events.

### Categories
Organisers can manage event categories used by RaceDay events.

### Enrolments
Participants can enrol in event categories and manage their own enrolments.

### Results
Organisers can record and update race results, while participants can view their own results.


## Access Control

Public endpoints can be accessed without authentication. Organiser endpoints are restricted to users with the Organiser role, while participant endpoints are restricted to users with the Participant role.