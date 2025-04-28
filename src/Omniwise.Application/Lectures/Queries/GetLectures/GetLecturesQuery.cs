using MediatR;
using Omniwise.Application.Lectures.Dtos;

namespace Omniwise.Application.Lectures.Queries.GetLectures;

public class GetLecturesQuery(int courseId) : IRequest<IEnumerable<LectureToGetAllDto>>
{
    public int CourseId { get; set; } = courseId;
}
