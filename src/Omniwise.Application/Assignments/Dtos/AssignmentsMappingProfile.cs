using AutoMapper;
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
            CreateMap<Assignment, AssignmentDto>();
        }
    }
}
