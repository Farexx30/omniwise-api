# 📚 Omniwise
Omniwise is a web-based Learning Management System designed to streamline course and assignment workflows in an educational setting. The application supports role-based access for teachers, students, and administrators, allowing for course creation, file-based assignment submissions, and automated notifications. Files are managed in Azure Blob Storage, and missed assignment deadlines are tracked via scheduled background jobs using Quartz.NET, keeping educators informed.


## 💡 Features
### 🔐 Role-based authorization
  - Teachers can:
    - Create and manage courses
    - Enroll to courses
    - Add or remove members from courses
    - Upload lecture materials with file attachments
    - Create assignments with file attachments and set deadlines
    - View, grade, and comment on student submissions
  - Students can:
    - Enroll in available courses
    - View lectures and assignments
    - Submit assignment files
    - Comment on their submissions
  - Administrators can:
    - Manage user accounts
    - Remove users from the system


    
### 🗂️ File management 
All lecture materials and assignment submissions are stored and retrieved using Azure Blob Storage, ensuring scalable and secure file handling. Files are organized by course and entity type
### 🔔 Notifications
  - On-action notifications: 
    Students and teachers receive notifications about new lectures, assignments and comments
  - Scheduled notifications: 
    After the assignment deadline, a background job powered by Quartz.NET notifies teachers about students who failed to submit their work on time.



## 🛠 Tech Stack
**Backend:** ASP.NET Core (REST API)

**Architecture:** Clean Architecture

**ORM:** Entity Framework Core

**Database:** MS SQL Server

**Storage:** Azure Data Studio, Azure Blob Storage

**Scheduler:** Quartz.NET

**Other:** FluentValidation, AutoMapper, MediatR



## 🚀 Getting Started

### 📌 Prerequisites

- .NET 9 SDK
- MS SQL Server
- Azure Blob Storage account (optional, for full functionality)
- Visual Studio or VS Code


> [!IMPORTANT]
> Do not forget to check the setup details to avoid setup errors.

### 🗃️ Setup

1. Clone the repository
2. Open the `Secrets.json` file:

![Omniwise.API -> Connected Services -> Secrets.json](https://github.com/user-attachments/assets/07c518b9-5f7e-44a2-96de-25b9fb04f069)

Add the admin login details using the following code:
```json
  "SeedAdmin": {
    "Email": "example@email.com",
    "FirstName": "FirstName",
    "LastName": "LastName",
    "Password": "Password"
  }
```

3. (optional) Update the `appsettings.Development.json` with your custom connection string
4. Run the application.

   
> [!TIP]
> After running the app, the API will be available at:
> - `https://localhost:7155/swagger` (Swagger UI)
> - `http://localhost:5096/swagger`
>
> These URLs are defined in `launchSettings.json`. If you change the ports, make sure to update this file accordingly.

## 📸 Sreenshots
![Authorization](https://github.com/user-attachments/assets/06799313-7f4d-4b41-9047-fff5a4c674c5)

![Assignments](https://github.com/user-attachments/assets/5048678d-9de7-41ab-a0e1-07ecb607fbb9)

![AssignmentSubmissions](https://github.com/user-attachments/assets/2a809009-9c22-4843-af12-d46a5a7c153d)

![AssignmentSubmissionComments](https://github.com/user-attachments/assets/d142f308-3c46-4d76-98f1-0bc2ca4f0e7f)

![Courses](https://github.com/user-attachments/assets/8bbd0282-8863-4e77-bc70-381771b90be3)

![PostCourse](https://github.com/user-attachments/assets/3c1e3f8b-fb09-462e-b7b4-8df9dd6efb84)

![PostCourseResponses](https://github.com/user-attachments/assets/2273ee2e-1640-4fc9-905e-f051dbca062a)

![CourseMembers](https://github.com/user-attachments/assets/e34f86f8-f0f9-4111-9c42-40f1401b5d07)

![Identity](https://github.com/user-attachments/assets/c19f3887-b5eb-4efc-ba1b-215e4883d033)

![Lectures](https://github.com/user-attachments/assets/d3e003ab-a702-476e-b1f7-d11f30b64bf4)

![Notifications](https://github.com/user-attachments/assets/7314b0f5-5e9a-4c4c-bb75-263c44ff4122)

![Users](https://github.com/user-attachments/assets/00147c16-dfae-4ec7-8c84-8321f8e555b5)









