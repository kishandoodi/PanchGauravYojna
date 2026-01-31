using BL;
using BL.Account;
using BL.Common;
using BL.Department;
using BL.FinancialYear;
using BL.GauravMaster;
using BL.GroupMaster;
using BL.Log;
using BL.ManageMaster;
using BL.PageAccessRequirement;
using BL.ProfileUser;
using BL.Progres;
using BL.WebsiteMaster;
using DL;
using Microsoft.AspNetCore.Authorization;
using PanchGauravYojna.Service;
using System.Data;
namespace PanchGauravYojna
{
    public class ServiceInject
    {
        public ServiceInject(IServiceCollection services)
        {
            services.AddScoped(typeof(ISQLHelper), typeof(SQLHelper));
            services.AddScoped(typeof(ILogin), typeof(Login));
            services.AddScoped(typeof(ICommon), typeof(Common));
            services.AddScoped(typeof(IProgressReport), typeof(ProgressReport));
            services.AddScoped(typeof(IInputValidator), typeof(InputValidator));
            services.AddScoped(typeof(IGauravProfileAnswer), typeof(GauravProfileAnswer));
            services.AddScoped(typeof(IProfileUser), typeof(ProfileUser));
            services.AddScoped(typeof(IMenu), typeof(Menu));
            services.AddScoped(typeof(IGroupMaster), typeof(GroupMaster));
            services.AddScoped(typeof(IPermission), typeof(Permission));
            services.AddScoped(typeof(IDepartment), typeof(Department));
            services.AddScoped(typeof(IGauravMaster), typeof(GauravMaster));
            services.AddScoped(typeof(ILog), typeof(Log));
            services.AddScoped(typeof(IGauravDistrict), typeof(GauravDistrict));
            services.AddScoped(typeof(IFinancialYear), typeof(FinancialYear));
            services.AddScoped(typeof(ISliderVM), typeof(SliderVM));
            services.AddScoped<IAuthorizationHandler, PageAccessHandler>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddHttpContextAccessor();          
        }
    }
}
