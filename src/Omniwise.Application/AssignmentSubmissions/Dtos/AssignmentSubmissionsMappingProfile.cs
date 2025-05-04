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

        CreateMap<AssignmentSubmission, AssignmentSubmissionDto>();
    }
}
