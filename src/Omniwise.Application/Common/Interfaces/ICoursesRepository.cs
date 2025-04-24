using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Common.Interfaces;

public interface ICoursesRepository
{
    Task<Course?> GetByIdAsync(int id);
    Task<int> CreateAsync(Course course);
    Task SaveChangesAsync();
}
