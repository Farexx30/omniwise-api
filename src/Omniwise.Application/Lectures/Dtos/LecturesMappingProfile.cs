using Omniwise.Domain.Entities;
using AutoMapper;
using Omniwise.Application.Lectures.Commands.CreateLecture;

namespace Omniwise.Application.Lectures.Dtos;

public class LecturesMappingProfile : Profile
{
    public LecturesMappingProfile()
    {
        CreateMap<Lecture, LectureDto>();
        CreateMap<CreateLectureCommand, Lecture>();
    }
}
