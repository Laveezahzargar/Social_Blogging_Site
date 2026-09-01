

namespace BlogGenerator.ServiceModels.v1;

public class AdminUserDetailsDto : AdminUserDto
{
    public int TotalBlogs { get; set; }
    public int TotalLikes { get; set; }
    public int TotalComments { get; set; }
    public int TotalPayments { get; set; }
}
