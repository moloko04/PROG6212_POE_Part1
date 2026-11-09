PROG6212 - RaceDay Event Management System

CI/CD Status
![CI Pipeline](https://github.com/moloko04/PROG6212_POE_Part1/actions/workflows/blank.yml/badge.svg)

Student Information
- Student Name: Moloko Dineo Mothemela
- Student Number: ST10460465
- Module: PROG6212

Project Overview
RaceDay is a full-stack web-based event management system designed for the South African road running, walking and cycling community. It consists of a RESTful API (Part 2) and an ASP.NET Core MVC front-end (Part 3).

Part 1 Deliverables
All Part 1 planning documents are in the `/docs` folder:
- Entity Relationship Diagram (ERD)
- API Endpoint Plan (Word and PDF)
- SQL Database Script

Part 2 Deliverables
Part 2 is the RESTful API built with ASP.NET Core Web API and Entity Framework Core.
- 23 endpoints covering Authentication, User Profile, Events, Categories, Enrolments, Results and Prizes.
- JWT Authentication and Role-based access control.
- 20 Unit tests covering success and failure scenarios.

Part 3 Deliverables
Part 3 is the ASP.NET Core MVC front-end that consumes the API.
- Participant Interface: View upcoming events, enrol, view personal results and update profile.
- Organiser Interface: Dashboard, create/manage events, manage categories, view enrolments and capture results.
- Session Management: Role-based navigation.
- Azure Blob Storage: Integrated into the API to handle event banner image uploads.
- Pink Sporting Theme: Custom CSS applied for a consistent, energetic brand feel.

Setup Instructions (Local Development)
1. Clone this repository.
2. Open `RaceDay.API.sln` in Visual Studio 2022.
3. Ensure both `RaceDay.API` and `RaceDay.Web` are set as startup projects.
4. Open Package Manager Console, select `RaceDay.API` and run `Update-Database`.
5. Start Azurite (local Azure emulator) in a command prompt: `azurite`.
6. Press F5 to run both projects.

Docker Instructions
The MVC application is containerized. To build and run it:

1. Open a terminal in the `RaceDay.Web` project folder.
2. Build the Docker image:
   ```bash
   docker build -t raceday-web -f Dockerfile .
3. docker run -d -p 8080:8080 --name raceday-web-container raceday-web
4. Access the application at: http://localhost:8080

YouTube Video
[Watch my explanation video here](https://youtu.be/1MYHwklq6FQ?si=vPlYT50bjRjb3_x1)

