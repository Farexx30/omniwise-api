using Microsoft.EntityFrameworkCore;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Repositories;

internal class CoursesRepository(OmniwiseDbContext dbContext) : ICoursesRepository
{
    public async Task<Course?> GetByIdAsync(int id)
    {
        var course = await dbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == id);

        return course;
    }

    public async Task<int> CreateAsync(Course course)
    {
        dbContext.Courses.Add(course);
        await dbContext.SaveChangesAsync();

        return course.Id;
    }

    public async Task DeleteAsync(Course course)
    {
        dbContext.Courses.Remove(course);
        await dbContext.SaveChangesAsync();
    }

    public Task SaveChangesAsync()
       => dbContext.SaveChangesAsync();
}
