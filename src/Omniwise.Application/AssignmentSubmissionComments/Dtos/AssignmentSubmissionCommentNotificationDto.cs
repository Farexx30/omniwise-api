namespace Omniwise.Application.AssignmentSubmissionComments.Dtos;

public class AssignmentSubmissionCommentNotificationDto
{
    public string CourseName { get; set; } = default!;
    public int CourseId { get; set; }
    public string AssignmentName { get; set; } = default!;
    public string AssignmentSubmissionAuthorFirstName { get; set; } = default!;
    public string AssignmentSubmissionAuthorLastName { get; set; } = default!;
    public string CommentAuthorFirstName { get; set; } = default!;
    public string CommentAuthorLastName { get; set; } = default!;
}
