using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Finbuckle.MultiTenant;
using EMS20.WebApi.Application.Common.Exceptions;
using EMS20.WebApi.Application.Common.Specification;
using EMS20.WebApi.Domain.Identity;
using EMS20.WebApi.Infrastructure.Auth;
using EMS20.WebApi.Infrastructure.Persistence.Context;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using EMS20.WebApi.Application.Common.Caching;
using EMS20.WebApi.Application.Common.Events;
using EMS20.WebApi.Application.Common.FileStorage;
using EMS20.WebApi.Application.Common.Interfaces;
using EMS20.WebApi.Application.Common.Mailing;
using EMS20.WebApi.Application.Common.Models;
using EMS20.WebApi.Application.Identity.Users;
using EMS20.WebApi.Shared.Authorization;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using StackExchange.Redis;

namespace EMS20.WebApi.Infrastructure.Identity;

internal partial class UserService : IUserService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _db;
    private readonly IStringLocalizer _t;

    private readonly IJobService _jobService;
    private readonly IMailService _mailService;
    private readonly SecuritySettings _securitySettings;
    private readonly IEmailTemplateService _templateService;
    private readonly IFileStorageService _fileStorage;
    private readonly IEventPublisher _events;
    private readonly ICacheService _cache;
    private readonly ICacheKeyService _cacheKeys;
    private readonly ITenantInfo _currentTenant;

    public UserService(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext db,
        IStringLocalizer<UserService> localizer,
        IJobService jobService,
        IMailService mailService,
        IEmailTemplateService templateService,
        IFileStorageService fileStorage,
        IEventPublisher events,
        ICacheService cache,
        ICacheKeyService cacheKeys,
        ITenantInfo currentTenant,
        IOptions<SecuritySettings> securitySettings)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
        _t = localizer;
        _jobService = jobService;
        _mailService = mailService;
        _templateService = templateService;
        _fileStorage = fileStorage;
        _events = events;
        _cache = cache;
        _cacheKeys = cacheKeys;
        _currentTenant = currentTenant;
        _securitySettings = securitySettings.Value;
    }

    public async Task<PaginationResponse<UserDetailsDto>> SearchAsync(UserListFilter filter, CancellationToken cancellationToken)
    {
        var spec = new EntitiesByPaginationFilterSpec<ApplicationUser>(filter);

        var users = await _userManager.Users
            .WithSpecification(spec)
            .ProjectToType<UserDetailsDto>()
            .ToListAsync(cancellationToken);
        int count = await _userManager.Users
            .CountAsync(cancellationToken);

        return new PaginationResponse<UserDetailsDto>(users, count, filter.PageNumber, filter.PageSize);
    }

    public async Task<bool> ExistsWithNameAsync(string name)
    {
        EnsureValidTenant();
        return await _userManager.FindByNameAsync(name) is not null;
    }

    public async Task<bool> ExistsWithEmailAsync(string email, string? exceptId = null)
    {
        EnsureValidTenant();
        return await _userManager.FindByEmailAsync(email.Normalize()) is ApplicationUser user && user.Id != exceptId;
    }

    public async Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, string? exceptId = null)
    {
        EnsureValidTenant();
        return await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber) is ApplicationUser user && user.Id != exceptId;
    }

    private void EnsureValidTenant()
    {
        if (string.IsNullOrWhiteSpace(_currentTenant?.Id))
        {
            throw new UnauthorizedException(_t["Invalid Tenant."]);
        }
    }

    public async Task<List<UserDetailsDto>> GetListAsync(CancellationToken cancellationToken) =>
        (await _userManager.Users
                .AsNoTracking()
                .ToListAsync(cancellationToken))
            .Adapt<List<UserDetailsDto>>();

    public Task<int> GetCountAsync(CancellationToken cancellationToken) =>
        _userManager.Users.AsNoTracking().CountAsync(cancellationToken);

    public async Task<UserDetailsDto> GetAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        return user.Adapt<UserDetailsDto>();
    }

    public async Task ToggleStatusAsync(ToggleUserStatusRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        bool isAdmin = await _userManager.IsInRoleAsync(user, FSHRoles.Admin);
        if (isAdmin)
        {
            throw new ConflictException(_t["Administrators Profile's Status cannot be toggled"]);
        }

        user.IsActive = request.ActivateUser;

        await _userManager.UpdateAsync(user);

        await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));
    }

    public async Task<ResultResponse<Dictionary<string, List<UserDetailsWithoutRoleDto>>>> ListOfUsersByRole(List<DefaultIdType> roleIds, CancellationToken cancellationToken)
    {
        var usersInRoles = new Dictionary<string, List<UserDetailsWithoutRoleDto>>();

        // Retrieve roles based on the list of role IDs
        var stringRoleIds = roleIds.Select(id => id.ToString());
        var roles = await _db.Roles.Where(x => stringRoleIds.Contains(x.Id)).ToListAsync(cancellationToken);

        foreach (var roleId in roleIds)
        {
            var role = roles.SingleOrDefault(x => x.Id == roleId.ToString());
            _ = role ?? throw new NotFoundException($"Role with ID {roleId} not found");

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

            // Convert users in the role to UserDetailsWithoutRoleDto
            var usersInRoleDto = usersInRole
                .Select(user => new UserDetailsWithoutRoleDto
                {
                    Id = new Guid(user.Id),
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumber = user.PhoneNumber,
                    ImageUrl = user.ImageUrl,
                })
                .ToList();

            // Add the users to the dictionary using the role name as the key
            usersInRoles.Add(role.Name, usersInRoleDto);
        }

        return new ResultResponse<Dictionary<string, List<UserDetailsWithoutRoleDto>>>
        {
            Data = usersInRoles,
            Message = "Request Executed Successfully",
            Success = true
        };
    }

    public async Task<List<ApplicationUser>> GetUsersByRolesAsync(List<string> roles)
    {
        var users = new List<ApplicationUser>();

        foreach (var roleName in roles)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            users.AddRange(usersInRole);
        }

        return users;
    }



}