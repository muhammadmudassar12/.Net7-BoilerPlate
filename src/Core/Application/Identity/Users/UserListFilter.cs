using EMS20.WebApi.Application.Common.Models;

namespace EMS20.WebApi.Application.Identity.Users;

public class UserListFilter : PaginationFilter
{
    public bool? IsActive { get; set; }
}