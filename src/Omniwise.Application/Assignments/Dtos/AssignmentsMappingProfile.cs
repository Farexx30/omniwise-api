using AutoMapper;
using Omniwise.Application.Assignments.Commands.CreateAssignment;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Assignments.Dtos
{
    public class AssignmentsMappingProfile : Profile
    {
        public AssignmentsMappingProfile()
        {
            CreateMap<CreateAssignmentCommand, Assignment>();
            CreateMap<Assignment, AssignmentDto>();
        }
    }
}
