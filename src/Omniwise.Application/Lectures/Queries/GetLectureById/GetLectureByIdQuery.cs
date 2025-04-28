using MediatR;
using Omniwise.Application.Lectures.Dtos;

namespace Omniwise.Application.Lectures.Queries.GetLectureById;

public class GetLectureByIdQuery(int courseId, int lectureId) : IRequest<LectureDto>
{
    public int CourseId { get; } = courseId;
    public int LectureId { get; } = lectureId;
}