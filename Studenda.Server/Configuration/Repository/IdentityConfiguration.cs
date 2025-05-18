using Microsoft.AspNetCore.Identity;

namespace Studenda.Server.Configuration.Repository;

public class IdentityConfiguration(IConfiguration configuration) : ConfigurationRepository(configuration)
{
    private const string SectionName = "Identity";

    private bool GetPasswordRequireDigit()
    {
        return Configuration
            .GetSection(SectionName)
            .GetSection("Password")
            .GetValue<bool>("RequireDigit");
    }

    private bool GetPasswordRequireLowercase()
    {
        return Configuration
            .GetSection(SectionName)
            .GetSection("Password")
            .GetValue<bool>("RequireLowercase");
    }

    private bool GetPasswordRequireUppercase()
    {
        return Configuration
            .GetSection(SectionName)
            .GetSection("Password")
            .GetValue<bool>("RequireUppercase");
    }

    private bool GetPasswordRequireNonAlphanumeric()
    {
        return Configuration
            .GetSection(SectionName)
            .GetSection("Password")
            .GetValue<bool>("RequireNonAlphanumeric");
    }

    private int GetPasswordRequiredLength()
    {
        var result = Configuration
            .GetSection(SectionName)
            .GetSection("Password")
            .GetValue<int>("RequiredLength");

        return HandleIntValue(result, "Password required length is invalid!");
    }

    private int GetPasswordRequiredUniqueChars()
    {
        var result = Configuration
            .GetSection(SectionName)
            .GetSection("Password")
            .GetValue<int>("RequiredUniqueChars");

        return HandleIntValue(result, "Password required unique chars is invalid!");
    }

    private bool GetUserRequireUniqueEmail()
    {
        return Configuration
            .GetSection(SectionName)
            .GetSection("User")
            .GetValue<bool>("RequireUniqueEmail");
    }

    public void ModifyOptions(IdentityOptions options)
    {
        options.Password.RequireDigit = GetPasswordRequireDigit();
        options.Password.RequireLowercase = GetPasswordRequireLowercase();
        options.Password.RequireUppercase = GetPasswordRequireUppercase();
        options.Password.RequireNonAlphanumeric = GetPasswordRequireNonAlphanumeric();
        options.Password.RequiredLength = GetPasswordRequiredLength();
        options.Password.RequiredUniqueChars = GetPasswordRequiredUniqueChars();
        options.User.RequireUniqueEmail = GetUserRequireUniqueEmail();
    }
}