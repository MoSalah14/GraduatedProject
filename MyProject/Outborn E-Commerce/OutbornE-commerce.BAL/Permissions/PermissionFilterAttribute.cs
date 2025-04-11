using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.DAL.Enums;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

public class PermissionFilterAttribute : ActionFilterAttribute
{
    private readonly string PermissionValue;
    private readonly string PermissionValue2;

    public Permission PermissionsEnum { get; set; }
    //public Permission PermissionsEnum2 { get; set; }

    public PermissionFilterAttribute(Permission PermissionsEnum)
    {
        PermissionValue = PermissionsEnum.ToString();     
        //PermissionValue2 = PermissionsEnum2.ToString();  
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var identityClaims = context.HttpContext.User.Identity as ClaimsIdentity;

       
        var claimsList = identityClaims?.Claims.Where(x => x.Type.Equals("permissions")).ToList();

        if (identityClaims != null && claimsList != null)
        {
            foreach (var claim in claimsList)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }

            if (!claimsList.Any(x => x.Value == PermissionValue ))
            {
              
                context.Result = new UnauthorizedObjectResult(new
                {
                    status = false,
                    Message = "You don't have permission to Access!"
                });
                return;
            }
        }
        else
        {
           
            context.Result = new UnauthorizedObjectResult(new
            {
                status = false,
                Message = "You don't have permission to Access!"
            });
            return;
        }

        await base.OnActionExecutionAsync(context, next);
    }
}
