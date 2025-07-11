using AutoMapper;
using Omniwise.Application.AssignmentSubmissions.Commands.CreateAssignmentSubmission;
using Omniwise.Application.AssignmentSubmissions.Commands.UpdateAssignmentSubmission;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissions.Dtos;

public class AssignmentSubmissionsMappingProfile : Profile
{
    public AssignmentSubmissionsMappingProfile()
    {
        CreateMap<CreateAssignmentSubmissionCommand, AssignmentSubmission>()
            .ForMember(dest => dest.Files, opt => opt.Ignore());

        CreateMap<UpdateAssignmentSubmissionCommand, AssignmentSubmission>()
            .ForMember(dest => dest.Files, opt => opt.Ignore());

        CreateMap<AssignmentSubmission, AssignmentSubmissionDto>()
            .ForMember(dest => dest.AuthorFullName,
                opt => opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"))
            .ForMember(dest => dest.AssignmentId,
                opt => opt.MapFrom(src => src.Assignment.Id))
            .ForMember(dest => dest.AssignmentName,
                opt => opt.MapFrom(src => src.Assignment.Name))
            .ForMember(dest => dest.MaxGrade,
                opt => opt.MapFrom(src => src.Assignment.MaxGrade))
            .ForMember(dest => dest.Deadline,
                opt => opt.MapFrom(src => src.Assignment.Deadline));

        CreateMap<AssignmentSubmission, BasicAssignmentSubmissionDto>()
            .ForMember(dest => dest.AuthorFullName, 
                opt => opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"));
    }
}
