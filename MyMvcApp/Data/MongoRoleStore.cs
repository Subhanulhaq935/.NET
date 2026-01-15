using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using MyMvcApp.Data;

namespace MyMvcApp.Data;

public class MongoRoleStore : IRoleStore<IdentityRole>
{
    private readonly IMongoCollection<RoleDocument> _roles;

    public MongoRoleStore(IMongoDbService mongoDb)
    {
        var database = mongoDb.GetDatabase();
        _roles = database.GetCollection<RoleDocument>("roles");
    }

    public async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken = default)
    {
        try
        {
            var doc = new RoleDocument
            {
                Id = Guid.NewGuid().ToString(),
                Name = role.Name ?? string.Empty,
                NormalizedName = role.NormalizedName ?? role.Name?.ToUpperInvariant() ?? string.Empty
            };
            await _roles.InsertOneAsync(doc, cancellationToken: cancellationToken);
            role.Id = doc.Id;
            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    public async Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken = default)
    {
        try
        {
            var doc = new RoleDocument
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                NormalizedName = role.NormalizedName ?? role.Name?.ToUpperInvariant() ?? string.Empty
            };
            var result = await _roles.ReplaceOneAsync(r => r.Id == role.Id, doc, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0 ? IdentityResult.Success : IdentityResult.Failed();
        }
        catch (Exception ex)
        {
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    public async Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken = default)
    {
        var result = await _roles.DeleteOneAsync(r => r.Id == role.Id, cancellationToken);
        return result.DeletedCount > 0 ? IdentityResult.Success : IdentityResult.Failed();
    }

    public async Task<IdentityRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken = default)
    {
        var doc = await _roles.Find(r => r.Id == roleId).FirstOrDefaultAsync(cancellationToken);
        return doc?.ToRole();
    }

    public async Task<IdentityRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        var doc = await _roles.Find(r => r.NormalizedName == normalizedRoleName).FirstOrDefaultAsync(cancellationToken);
        return doc?.ToRole();
    }

    public Task<string?> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken = default)
        => Task.FromResult(role.NormalizedName ?? role.Name?.ToUpperInvariant());

    public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken = default)
        => Task.FromResult(role.Id);

    public Task<string?> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken = default)
        => Task.FromResult(role.Name);

    public Task SetNormalizedRoleNameAsync(IdentityRole role, string? normalizedName, CancellationToken cancellationToken = default)
    {
        role.NormalizedName = normalizedName;
        return Task.CompletedTask;
    }

    public Task SetRoleNameAsync(IdentityRole role, string? roleName, CancellationToken cancellationToken = default)
    {
        role.Name = roleName;
        return Task.CompletedTask;
    }

    public void Dispose() { }
}

// Extension method for RoleDocument
public static class RoleDocumentExtensions
{
    public static IdentityRole ToRole(this RoleDocument doc)
    {
        return new IdentityRole
        {
            Id = doc.Id,
            Name = doc.Name,
            NormalizedName = doc.NormalizedName
        };
    }
}
