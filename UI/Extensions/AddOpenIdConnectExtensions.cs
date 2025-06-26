using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace FlazorTemplate.Extensions;

//
// Summary:
//     Extension methods on Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectOptions
public static class AddOpenIdConnectExtensions
{
    //
    // Summary:
    //     Enriches the ClaimsPrincipal representing the user with information about which
    //     groups they are a member of.
    //
    // Parameters:
    //   options:
    //     The options to register event callbacks against.
    //
    //   groupProvider:
    //     Provides group data.
    //
    //   role:
    //     The role the user must have in that group.
    public static void AddCustomerGroupDataToPrincipal(this OpenIdConnectOptions options, IGroupProvider groupProvider, string role)
    {
        OpenIdConnectEvents events = options.Events;
        events.OnUserInformationReceived = (Func<UserInformationReceivedContext, Task>)Delegate.Combine(events.OnUserInformationReceived, (Func<UserInformationReceivedContext, Task>)delegate (UserInformationReceivedContext context)
        {
            ClaimsIdentity identity = new ClaimsIdentity(groupProvider.GetGroups(context.Principal, context.Properties.Items[".Token.access_token"].ToString(), role).Result.Select(delegate (IGroup t)
            {
                Claim claim = new Claim("customer", t.Id.ToString())
                {
                    Properties =
                    {
                        { "Name", t.Name },
                        { "Description", t.Description },
                        {
                            "Id",
                            t.Id.ToString()
                        },
                        {
                            "ParentGroupId",
                            (!t.ParentGroupId.HasValue) ? string.Empty : t.ParentGroupId.ToString()
                        }
                    }
                };
                foreach (KeyValuePair<string, string> property in t.Properties)
                {
                    if (!claim.Properties.ContainsKey(property.Key))
                    {
                        claim.Properties.Add(property.Key, property.Value);
                    }
                }

                return claim;
            }));
            context.Principal.AddIdentity(identity);
            return Task.CompletedTask;
        });
    }
}