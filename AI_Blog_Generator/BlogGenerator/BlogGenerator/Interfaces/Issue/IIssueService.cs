using BlogGenerator.ServiceModels.v1.Issue;

namespace BlogGenerator.Interfaces
{
    public interface IIssueService
    {
        Task<IssueDto> CreateIssueAsync(
            int userId,
            CreateIssueDto dto);

        Task<List<IssueDto>> GetMyIssuesAsync(
            int userId);

        Task<IssueDto?> GetIssueByIdAsync(
            int issueId,
            int userId);

        Task<IssueDto?> UpdateIssueAsync(
            int issueId,
            int userId,
            UpdateIssueDto dto);

        Task<bool> DeleteIssueAsync(
            int issueId,
            int userId);

        Task<IssueDto?> GetIssueStatusAsync(
            int issueId,
            int userId);
    }
}