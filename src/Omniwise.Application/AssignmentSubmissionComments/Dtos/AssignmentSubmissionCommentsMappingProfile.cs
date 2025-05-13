using AutoMapper;
using Omniwise.Application.AssignmentSubmissionComments.Commands.CreateAssignmentSubmissionComment;
using Omniwise.Application.AssignmentSubmissionComments.Commands.UpdateAssignmentSubmissionComment;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissionComments.Dtos;

public class AssignmentSubmissionCommentsMappingProfile : Profile
{
    public AssignmentSubmissionCommentsMappingProfile()
    {
        CreateMap<CreateAssignmentSubmissionCommentCommand, AssignmentSubmissionComment>();
        CreateMap<UpdateAssignmentSubmissionCommentCommand, AssignmentSubmissionComment>();

        CreateMap<AssignmentSubmissionComment, AssignmentSubmissionCommentDto>()
            .ForMember(dest => dest.AuthorFullName, 
                       opt => opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"));
    }
}
