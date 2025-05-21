using MediatR;
using Omniwise.Application.Lectures.Dtos;

namespace Omniwise.Application.Lectures.Queries.GetAllCourseLectures;

public class GetAllCourseLecturesQuery : IRequest<IEnumerable<LectureToGetAllDto>>
{
    public required int CourseId { get; init; }
}
