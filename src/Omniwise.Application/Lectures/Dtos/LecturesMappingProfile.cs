using Omniwise.Domain.Entities;
using AutoMapper;

namespace Omniwise.Application.Lectures.Dtos;

public class LecturesMappingProfile : Profile
{
    public LecturesMappingProfile()
    {
        CreateMap<Lecture, LectureDto>();
    }
}
