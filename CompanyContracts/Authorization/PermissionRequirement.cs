﻿using Microsoft.AspNetCore.Authorization;

namespace CompanyContracts.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; private set; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}