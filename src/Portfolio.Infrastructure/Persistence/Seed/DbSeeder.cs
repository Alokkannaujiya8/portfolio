using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Portfolio.Domain.Entities;

using Portfolio.Infrastructure.Persistence.Context;

namespace Portfolio.Infrastructure.Persistence.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(PortfolioDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Force-refresh the database content by clearing old data if present
        if (context.Skills.Any(s => s.Name == "Git & Version Control") || !context.Skills.Any())
        {
            context.Heroes.RemoveRange(context.Heroes);
            context.Abouts.RemoveRange(context.Abouts);
            context.Projects.RemoveRange(context.Projects);
            context.Skills.RemoveRange(context.Skills);
            context.Experiences.RemoveRange(context.Experiences);
            context.Educations.RemoveRange(context.Educations);
            await context.SaveChangesAsync();
        }

        // 1. Seed Roles
        var adminRoleName = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRoleName))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRoleName));
        }

        // 2. Seed Admin User
        var adminEmail = "alokkanojiya96@gmail.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, "Alok@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, adminRoleName);
            }
        }

        // 3. Seed Hero
        var hero = context.Heroes.FirstOrDefault();
        if (hero == null)
        {
            context.Heroes.Add(new Hero
            {
                Title = "Hi, I'm Alok",
                Subtitle = "ASP.NET Developer",
                ImageUrl = "uploads/Alok.jpeg",
                ResumeUrl = "https://drive.google.com/uc?export=download&id=1lBua6KF4OEO5Hb2PYLT16bPSXdedY_0c",
                PrimaryButtonText = "View Projects",
                SecondaryButtonText = "Contact Me"
            });
        }
        else if (string.IsNullOrEmpty(hero.ImageUrl) || hero.ImageUrl == "uploads/profile.jpg")
        {
            hero.ImageUrl = "uploads/Alok.jpeg";
            context.Heroes.Update(hero);
        }

        // 4. Seed About
        var about = context.Abouts.FirstOrDefault();
        if (about == null)
        {
            context.Abouts.Add(new About
            {
                Title = "About Me",
                Subtitle = "Designing and building scalable enterprise web applications",
                Description = "Motivated ASP.NET Developer with 11 months of hands-on industry experience building scalable web applications using ASP.NET Core, RESTful APIs, and Angular. Passionate about clean architecture and contributing to high-impact software projects. Seeking a challenging role to grow technically and deliver production-grade solutions.",
                ImageUrl = "uploads/Alok.jpeg",
                Location = "New Delhi",
                Email = "alokkanojiya96@gmail.com",
                Phone = "8299078491",
                ExperienceYears = 1,
                ProjectsCompleted = 3
            });
        }
        else if (string.IsNullOrEmpty(about.ImageUrl) || about.ImageUrl == "uploads/profile.jpg")
        {
            about.ImageUrl = "uploads/Alok.jpeg";
            about.Description = "Motivated ASP.NET Developer with 11 months of hands-on industry experience building scalable web applications using ASP.NET Core, RESTful APIs, and Angular. Passionate about clean architecture and contributing to high-impact software projects. Seeking a challenging role to grow technically and deliver production-grade solutions.";
            context.Abouts.Update(about);
        }

        // 5. Seed Projects
        var chatProject = context.Projects.FirstOrDefault(p => p.Title == "Real-Time Chat Application");
        if (chatProject != null)
        {
            context.Projects.Remove(chatProject);
        }

        if (!context.Projects.Any())
        {
            context.Projects.AddRange(new List<Project>
            {
                new Project
                {
                    Title = "Human Resource Management System (HRMS)",
                    Description = "Developed a full-stack HRMS with modules for employee onboarding, attendance tracking, leave management, and payroll reporting. Implemented role-based access control (RBAC) with Admin, HR Manager, and Employee roles. Built PDF/Excel reporting features for attendance and performance summaries. Integrated centralized logging for audit trails and system monitoring.",
                    ImageUrl = "uploads/projects/hrms.jpg",
                    ProjectUrl = "https://hrms.example.com",
                    GithubUrl = "https://github.com/Alokkannaujiya8",
                    Technologies = "ASP.NET Core Web API, Angular, SQL Server",
                    DisplayOrder = 1,
                    IsFeatured = true
                },
                new Project
                {
                    Title = "E-Commerce Management System",
                    Description = "Developed product listing, cart, and order management modules. Implemented lazy loading, route guards, and REST APIs for product and order operations.",
                    ImageUrl = "uploads/projects/ecommerce.jpg",
                    ProjectUrl = "https://ecommerce.example.com",
                    GithubUrl = "https://github.com/Alokkannaujiya8",
                    Technologies = "Angular, TypeScript, ASP.NET Core Web API, SQL Server",
                    DisplayOrder = 2,
                    IsFeatured = true
                }
            });
        }

        // 6. Seed Skills
        if (!context.Skills.Any())
        {
            context.Skills.AddRange(new List<Skill>
            {
                // Backend Skills
                new Skill { Name = "ASP.NET Core / Web API / MVC", Category = "Backend", Proficiency = 90, DisplayOrder = 1 },
                new Skill { Name = "C# / Entity Framework Core", Category = "Backend", Proficiency = 88, DisplayOrder = 2 },
                new Skill { Name = "RESTful API Development", Category = "Backend", Proficiency = 85, DisplayOrder = 3 },
                new Skill { Name = "Clean Architecture", Category = "Backend", Proficiency = 85, DisplayOrder = 4 },
                new Skill { Name = "ASP.NET Web Forms", Category = "Backend", Proficiency = 70, DisplayOrder = 5 },

                // Frontend Skills
                new Skill { Name = "Angular / TypeScript", Category = "Frontend", Proficiency = 85, DisplayOrder = 6 },
                new Skill { Name = "HTML5 / CSS3", Category = "Frontend", Proficiency = 85, DisplayOrder = 7 },

                // Database Skills
                new Skill { Name = "SQL Server", Category = "Database", Proficiency = 85, DisplayOrder = 8 },
                new Skill { Name = "MongoDB", Category = "Database", Proficiency = 75, DisplayOrder = 9 },

                // Tools & Other Skills
                new Skill { Name = "Git", Category = "Tools & Other", Proficiency = 90, DisplayOrder = 10 },
                new Skill { Name = "Visual Studio / VS Code", Category = "Tools & Other", Proficiency = 90, DisplayOrder = 11 },
                new Skill { Name = "Postman", Category = "Tools & Other", Proficiency = 85, DisplayOrder = 12 }
            });
        }

        // 7. Seed Experience
        var jogazExp = context.Experiences.FirstOrDefault(e => e.Company.Contains("Jogaz Info"));
        if (jogazExp != null)
        {
            jogazExp.StartDate = new DateTime(2025, 8, 1);
            context.Experiences.Update(jogazExp);
        }
        else if (!context.Experiences.Any())
        {
            context.Experiences.AddRange(new List<Experience>
            {
                new Experience
                {
                    Company = "Jogaz Info Pvt. Ltd",
                    Role = "ASP.NET Developer",
                    Location = "New Delhi, India",
                    StartDate = new DateTime(2025, 8, 1),
                    IsCurrent = true,
                    Description = "• Developing and maintaining web applications using ASP.NET Core Web API and ASP.NET Core MVC.\n• Building and consuming RESTful APIs integrated with Angular front-end components.\n• Designing and optimizing SQL Server database schemas, queries, and stored procedures.\n• Implementing role-based authentication and authorization using ASP.NET Identity.\n• Writing clean, maintainable code following Clean Architecture principles and SOLID design patterns.\n• Participating in code reviews, debugging, and performance optimization of existing modules."
                }
            });
        }

        // 8. Seed Education
        if (!context.Educations.Any())
        {
            context.Educations.AddRange(new List<Education>
            {
                new Education
                {
                    Institution = "HLM Group of Institutions, Ghaziabad (AKTU, Lucknow)",
                    Degree = "Master of Computer Applications (MCA)",
                    FieldOfStudy = "Computer Science & Engineering",
                    StartDate = new DateTime(2025, 1, 1),
                    EndDate = new DateTime(2027, 1, 1),
                    Grade = "Pursuing",
                    IsCurrent = true
                },
                new Education
                {
                    Institution = "Adhunik Institute of Education and Research, Ghaziabad (CCS University, Meerut)",
                    Degree = "Bachelor of Computer Applications (BCA)",
                    FieldOfStudy = "Computer Science & Engineering",
                    StartDate = new DateTime(2022, 1, 1),
                    EndDate = new DateTime(2025, 1, 1),
                    Grade = "Completed",
                    IsCurrent = false
                }
            });
        }

        // 9. Seed Certificates
        if (!context.Certificates.Any())
        {
            context.Certificates.AddRange(new List<Certificate>
            {
                new Certificate
                {
                    Name = "ASP.NET Core Web API & Angular Integration",
                    Issuer = "Udemy",
                    IssueDate = new DateTime(2025, 5, 1),
                    CredentialId = "UD-ASP-ANG-99",
                    CredentialUrl = "https://udemy.com"
                }
            });
        }




        // 10. Seed Services
        if (!context.Services.Any())
        {
            context.Services.AddRange(new List<Service>
            {
                new Service { Title = "Backend Web API Development", Description = "Designing high-performance, secure RESTful APIs using ASP.NET Core and Entity Framework Core.", Icon = "api" },
                new Service { Title = "Frontend SPA Development", Description = "Building responsive, modern, and interactive single page applications using Angular.", Icon = "computer" },
                new Service { Title = "Database Administration", Description = "Designing, query-tuning, and maintaining relational databases with SQL Server.", Icon = "storage" }
            });
        }

        // 11. Seed Blogs
        if (!context.Blogs.Any())
        {
            context.Blogs.AddRange(new List<Blog>
            {
                new Blog
                {
                    Title = "Building Scalable RESTful APIs with ASP.NET Core 9",
                    Slug = "building-scalable-restful-apis-aspnet-9",
                    Summary = "Best practices for designing, securing, and testing enterprise Web APIs in ASP.NET Core 9.",
                    Content = "<p>In this post, we discuss API optimization, middleware registration, dependency injection, and clean responses in ASP.NET Core 9.</p>",
                    ImageUrl = "uploads/blogs/webapi.jpg",
                    IsPublished = true,
                    PublishedAt = DateTime.UtcNow
                },
                new Blog
                {
                    Title = "Structuring Angular 20 Apps with traditional NgModules",
                    Slug = "structuring-angular-20-apps-ngmodules",
                    Summary = "A complete walkthrough on why and how to organize large Angular applications into feature modules.",
                    Content = "<p>Traditional NgModules provide modular boundaries. We discuss SharedModule, CoreModule, and feature-based routing configuration.</p>",
                    ImageUrl = "uploads/blogs/angular-modules.jpg",
                    IsPublished = true,
                    PublishedAt = DateTime.UtcNow
                }
            });
        }

        // 12. Seed Settings
        if (!context.Settings.Any())
        {
            context.Settings.AddRange(new List<Setting>
            {
                new Setting { Key = "SiteName", Value = "Alok's Professional Portfolio" },
                new Setting { Key = "GitHub", Value = "https://github.com/Alokkannaujiya8" },
                new Setting { Key = "LinkedIn", Value = "https://linkedin.com/in/alok8" },
                new Setting { Key = "Email", Value = "alokkanojiya96@gmail.com" }
            });
        }

        // 13. Seed SEOs
        if (!context.SEOs.Any())
        {
            context.SEOs.AddRange(new List<Domain.Entities.SEO>
            {
                new Domain.Entities.SEO { PageName = "Home", Title = "Alok - ASP.NET Developer Portfolio", Description = "Welcome to my portfolio showcasing scalable software solutions in ASP.NET Core and Angular.", Keywords = "developer, .net, angular, portfolio, Alok" },
                new Domain.Entities.SEO { PageName = "Blogs", Title = "Alok's Tech Corner", Description = "Articles about ASP.NET Core, Angular, SQL Server, and Clean Architecture.", Keywords = "blog, coding, dotnet, angular" },
                new Domain.Entities.SEO { PageName = "Contact", Title = "Connect with Alok", Description = "Reach out for development inquiries or full-time opportunities.", Keywords = "contact, hire, developer" }
            });
        }

        // 14. Seed Resumes
        if (!context.Resumes.Any())
        {
            context.Resumes.Add(new Resume
            {
                FileName = "Alok_Resume.pdf",
                FilePath = "uploads/Alok_Resume.pdf",
                IsActive = true
            });
        }

        await context.SaveChangesAsync();
    }
}
