using Omniwise.Domain.Entities;
using AutoMapper;
using Omniwise.Application.Lectures.Commands.CreateLecture;
using Omniwise.Application.Lectures.Commands.UpdateLecture;

namespace Omniwise.Application.Lectures.Dtos;

public class LecturesMappingProfile : Profile
{
    public LecturesMappingProfile()
    {
        CreateMap<Lecture, LectureDto>();
        CreateMap<Lecture, LectureToGetAllDto>();

        CreateMap<CreateLectureCommand, Lecture>()
            .ForMember(dest => dest.Files, opt => opt.Ignore());

        CreateMap<UpdateLectureCommand, Lecture>()
            .ForMember(dest => dest.Files, opt => opt.Ignore());
    }
}
