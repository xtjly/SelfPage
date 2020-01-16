using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SelfPage_Service.Service;
using SelfPage_TestWebAPI.Filter;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace SelfPage_TestWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddAuthentication("Bearer").AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidAudience = "SelfPage",
                    ValidIssuer = "SelfPage",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("selfpagekey1234567890"))
                };
            });
            services.AddMvc(
                option =>
                {
                    option.Filters.Add<HttpGlobalExceptionFilter>();
                }
                ).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors(option =>
            {
                option.AddPolicy("AllowAllOrigin", builder =>
                {
                    builder.AllowCredentials()
                    .WithMethods("get", "post")
                    .AllowAnyHeader()
                    .AllowAnyOrigin()
                    //.WithHeaders()
                    //.WithOrigins()
                    ;
                });
            });
            services.Configure<FormOptions>(option =>
            {
                option.ValueLengthLimit = int.MaxValue;
                option.MultipartBodyLengthLimit = int.MaxValue;
                option.MultipartHeadersLengthLimit = int.MaxValue;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseSelfPage(
                //SelfPage配置信息
                opt =>
                {
                    //opt.EndPointPath = "/selfpage";
                    opt.AddAuthorizationHeader = true;
                    opt.IfUseXmlInfo = true;
                    opt.Groups.AddRange(new List<string> { "manage", "v1", "v2" });
                }
                ,
                //自定义分组策略，默认无分组策略
                (groupName, controllerInfo) =>
                {
                    return controllerInfo.ControllerRoute.StartsWith($"{groupName}");
                }
            );

            app.UseHttpsRedirection();
            app.UseCors("AllowAllOrigin");
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
