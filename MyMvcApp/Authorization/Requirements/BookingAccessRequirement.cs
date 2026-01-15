using Microsoft.AspNetCore.Authorization;

namespace MyMvcApp.Authorization.Requirements;

public class BookingAccessRequirement : IAuthorizationRequirement
{
    public bool RequireOwner { get; }

    public BookingAccessRequirement(bool requireOwner = true)
    {
        RequireOwner = requireOwner;
    }
}





