using MediatR;
using Omniwise.Application.Lectures.Dtos;

namespace Omniwise.Application.Lectures.Queries.GetLectureById;

public class GetLectureByIdQuery: IRequest<LectureDto>
{
    public required int LectureId { get; init; }
}