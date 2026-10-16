PROG6212 - RaceDay Event Management System

CI/CD Status
![CI Pipeline](https://github.com/moloko04/PROG6212_POE_Part1/actions/workflows/dotnet-tests.yml/badge.svg)

Student Information
- Student Name: Moloko Dineo Mothemela
- Student Number: ST10460465
- Module: PROG6212

Project Overview
RaceDay is a full-stack web-based event management system designed for the South African road running, walking, and cycling community.

Part 1 Deliverables
All Part 1 planning documents are in the `/docs` folder:
- Entity Relationship Diagram (ERD)
- API Endpoint Plan (Word and PDF)
- SQL Database Script

Part 2 Deliverables
Part 2 is the RESTful API built with ASP.NET Core Web API and Entity Framework Core.

Roles
- Organiser: Can create, update, and delete events, categories and prizes. Can capture results for participants. Can view enrolments for their own events.
- Participant: Can browse events, enrol in events, view their own enrolments and view their own results.

API Endpoints
The API includes 23 endpoints covering:
- Authentication (register, login)
- User Profile
- Events (CRUD)
- Categories (CRUD)
- Enrolments
- Results
- Prizes

All endpoints are documented and testable through Swagger.

Setup Instructions
1. Clone this repository.
2. Open `RaceDay.API.sln` in Visual Studio 2022.
3. Update the connection string in `appsettings.json` if needed.
4. Open Package Manager Console and run `Update-Database`.
5. Press F5 to run the API.
6. Swagger will open at `https://localhost:xxxx/swagger`.

Running Tests
1. In Visual Studio, go to Test - Run All Tests.
2. All 20 tests should pass.

CI/CD
This project uses GitHub Actions to automatically build the project and run all unit tests on every push to the main branch.

YouTube Video
[Watch my explanation video here]()
