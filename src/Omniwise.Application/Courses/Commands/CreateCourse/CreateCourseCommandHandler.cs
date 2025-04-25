using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Courses.Commands.CreateCourse
{
    public class CreateCourseCommandHandler(ICoursesRepository coursesRepository,
        ILogger<CreateCourseCommandHandler> logger,
        IMapper mapper,
        IUserContext userContext) : IRequestHandler<CreateCourseCommand, int>
    {
        public async Task<int> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();

            logger.LogInformation("Creating a new course {@request} by teacher with id: {currentUserId}", 
                request, 
                currentUser.Id);

            var course = mapper.Map<Course>(request);
            course.OwnerId = currentUser.Id!;

            var courseId = await coursesRepository.CreateAsync(course);

            return courseId;
        }
    }
}
