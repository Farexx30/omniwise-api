using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Common.Interfaces.Identity;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.Common.Interfaces.Storage;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Courses.Commands.CreateCourse
{
    public class CreateCourseCommandHandler(ICoursesRepository coursesRepository,
        IUserCourseRepository userCourseRepository,
        ILogger<CreateCourseCommandHandler> logger,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorageService,
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

            int courseId = 0;
            await unitOfWork.ExecuteTransactionalAsync(async () =>
            {
                courseId = await coursesRepository.CreateAsync(course);

                //After course we add the owner as a member with IsAccepted status equals true.
                var newUserInCourse = new UserCourse
                {
                    CourseId = courseId,
                    UserId = currentUser.Id!,
                    JoinDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    IsAccepted = true
                };

                await userCourseRepository.AddCourseMemberAsync(newUserInCourse);

                var courseImg = request.Img;
                if (courseImg is not null)
                {
                    var blobName = $"{FileFolders.CourseImages}/{courseId}-{courseImg.FileName}";

                    using var stream = courseImg.OpenReadStream();
                    await blobStorageService.UploadBlobAsync(stream, blobName);

                    course.ImgBlobName = blobName;

                    await coursesRepository.SaveChangesAsync();
                }
            });

            return courseId;
        }
    }
}
