using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using MyMvcApp.Models;

namespace MyMvcApp.Data;

public class MongoUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserEmailStore<ApplicationUser>, IUserRoleStore<ApplicationUser>, IQueryableUserStore<ApplicationUser>
{
    private readonly IMongoCollection<UserDocument> _users;
    private readonly IMongoCollection<RoleDocument> _roles;
    private readonly IMongoCollection<UserRoleDocument> _userRoles;

    public MongoUserStore(IMongoDbService mongoDb)
    {
        var database = mongoDb.GetDatabase();
        _users = database.GetCollection<UserDocument>("users");
        _roles = database.GetCollection<RoleDocument>("roles");
        _userRoles = database.GetCollection<UserRoleDocument>("userRoles");
    }

    // IUserStore implementation
    public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        try
        {
            var doc = UserDocument.FromUser(user);
            await _users.InsertOneAsync(doc, cancellationToken: cancellationToken);
            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        try
        {
            var doc = UserDocument.FromUser(user);
            var result = await _users.ReplaceOneAsync(u => u.Id == user.Id, doc, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0 ? IdentityResult.Success : IdentityResult.Failed();
        }
        catch (Exception ex)
        {
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        var result = await _users.DeleteOneAsync(u => u.Id == user.Id, cancellationToken);
        return result.DeletedCount > 0 ? IdentityResult.Success : IdentityResult.Failed();
    }

    public async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var doc = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync(cancellationToken);
        return doc?.ToUser();
    }

    public async Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
    {
        var doc = await _users.Find(u => u.NormalizedUserName == normalizedUserName).FirstOrDefaultAsync(cancellationToken);
        return doc?.ToUser();
    }

    public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        => Task.FromResult(user.NormalizedUserName ?? user.UserName?.ToUpperInvariant());

    public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        => Task.FromResult(user.Id);

    public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        => Task.FromResult(user.UserName);

    public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken = default)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken = default)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }

    // IUserPasswordStore
    public Task<string?> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        => Task.FromResult(user.PasswordHash);

    public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        => Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));

    public Task SetPasswordHashAsync(ApplicationUser user, string? passwordHash, CancellationToken cancellationToken = default)
    {
        user.PasswordHash = passwordHash;
        return Task.CompletedTask;
    }

    // IUserEmailStore
    public async Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        var doc = await _users.Find(u => u.NormalizedEmail == normalizedEmail).FirstOrDefaultAsync(cancellationToken);
        return doc?.ToUser();
    }

    public Task<string?> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        => Task.FromResult(user.Email);

    public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        => Task.FromResult(user.EmailConfirmed);

    public Task<string?> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        => Task.FromResult(user.NormalizedEmail ?? user.Email?.ToUpperInvariant());

    public Task SetEmailAsync(ApplicationUser user, string? email, CancellationToken cancellationToken = default)
    {
        user.Email = email;
        return Task.CompletedTask;
    }

    public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken = default)
    {
        user.EmailConfirmed = confirmed;
        return Task.CompletedTask;
    }

    public Task SetNormalizedEmailAsync(ApplicationUser user, string? normalizedEmail, CancellationToken cancellationToken = default)
    {
        user.NormalizedEmail = normalizedEmail;
        return Task.CompletedTask;
    }

    // IUserRoleStore
    public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken = default)
    {
        var role = await _roles.Find(r => r.NormalizedName == roleName.ToUpperInvariant()).FirstOrDefaultAsync(cancellationToken);
        if (role == null) return;

        var userRole = new UserRoleDocument { UserId = user.Id, RoleId = role.Id };
        await _userRoles.InsertOneAsync(userRole, cancellationToken: cancellationToken);
    }

    public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken = default)
    {
        var role = await _roles.Find(r => r.NormalizedName == roleName.ToUpperInvariant()).FirstOrDefaultAsync(cancellationToken);
        if (role == null) return;

        await _userRoles.DeleteOneAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id, cancellationToken);
    }

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        var userRoleIds = await _userRoles.Find(ur => ur.UserId == user.Id).Project(ur => ur.RoleId).ToListAsync(cancellationToken);
        var roles = await _roles.Find(r => userRoleIds.Contains(r.Id)).ToListAsync(cancellationToken);
        return roles.Select(r => r.Name).ToList();
    }

    public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken = default)
    {
        var role = await _roles.Find(r => r.NormalizedName == roleName.ToUpperInvariant()).FirstOrDefaultAsync(cancellationToken);
        if (role == null) return false;

        var userRole = await _userRoles.Find(ur => ur.UserId == user.Id && ur.RoleId == role.Id).FirstOrDefaultAsync(cancellationToken);
        return userRole != null;
    }

    public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var role = await _roles.Find(r => r.NormalizedName == roleName.ToUpperInvariant()).FirstOrDefaultAsync(cancellationToken);
        if (role == null) return new List<ApplicationUser>();

        var userIds = await _userRoles.Find(ur => ur.RoleId == role.Id).Project(ur => ur.UserId).ToListAsync(cancellationToken);
        var userDocs = await _users.Find(u => userIds.Contains(u.Id)).ToListAsync(cancellationToken);
        return userDocs.Select(d => d.ToUser()!).ToList();
    }

    // IQueryableUserStore
    public IQueryable<ApplicationUser> Users
    {
        get
        {
            // Load all users from MongoDB and return as IQueryable
            // Note: This loads all users into memory, which is fine for small to medium datasets
            var docs = _users.Find(_ => true).ToList();
            return docs.Select(d => d.ToUser()).Where(u => u != null).AsQueryable();
        }
    }

    public void Dispose() { }
}

public class UserDocument
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string NormalizedUserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }

    public static UserDocument FromUser(ApplicationUser user)
    {
        return new UserDocument
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            NormalizedUserName = user.NormalizedUserName ?? user.UserName?.ToUpperInvariant() ?? string.Empty,
            Email = user.Email ?? string.Empty,
            NormalizedEmail = user.NormalizedEmail ?? user.Email?.ToUpperInvariant() ?? string.Empty,
            EmailConfirmed = user.EmailConfirmed,
            PasswordHash = user.PasswordHash ?? string.Empty,
            Name = user.Name,
            Role = user.Role,
            IsApproved = user.IsApproved,
            CreatedAt = user.CreatedAt
        };
    }

    public ApplicationUser ToUser()
    {
        return new ApplicationUser
        {
            Id = Id,
            UserName = UserName,
            NormalizedUserName = NormalizedUserName,
            Email = Email,
            NormalizedEmail = NormalizedEmail,
            EmailConfirmed = EmailConfirmed,
            PasswordHash = PasswordHash,
            Name = Name,
            Role = Role,
            IsApproved = IsApproved,
            CreatedAt = CreatedAt
        };
    }
}

public class RoleDocument
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
}

public class UserRoleDocument
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
}
