using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using TechnicalCore.Models;
using TechnicalCore.Context;
using Microsoft.EntityFrameworkCore;
using TechnicalCore;
using TechnicalCore.Interfaces;
using TechnicalCore.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TechnicalWeb.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TechnicalWeb
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
            services.Configure<AppSetting>(Configuration.GetSection("AppSetting"));
            services.AddDbContext<DbLeonContext>(options => options.UseSqlServer(Configuration.GetConnectionString("EFCoreDBFirstDemoDatabase")));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAuthentication(
                //CookieAuthenticationDefaults.AuthenticationScheme
                options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }
                ).AddCookie().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = false,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = Configuration["Jwt:Issuer"],
                     ValidAudience = Configuration["Jwt:Issuer"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                 };
             });

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddCookie().AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = false,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = Configuration["Jwt:Issuer"],
            //        ValidAudience = Configuration["Jwt:Issuer"],
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
            //    };
            //});
            services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(30); });
            services.AddMvc();

            services.AddTransient<IExamSessionManager, ExamSessionManager>();
            services.AddTransient<ITechinicalTest, TechinicalTestManager>();
            services.AddTransient<IAccountManager, AccountManager>();
            services.AddTransient<IItemManager, ItemManager>();
            services.AddTransient<IListManager, ListManager>();
            services.AddTransient<IExamManager, ExamManager>();
            services.AddTransient<IExamQuestionManager, ExamQuestionManager>();
            services.AddTransient<IExamAnswerManager, ExamAnswerManager>();
            services.AddTransient<IExamStatusManager, ExamStatusManager>();
            services.AddTransient<JwtAuthentication, JwtAuthentication>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                //app.UseBrowserLink();
                //app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseCors(builder =>
       builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            //app.UseCookieAuthentication(new CookieAuthenticationOptions() { AuthenticationScheme="Test" });
            //app.UseCookieAuthentication(new CookieAuthenticationOptions()
            //{
            //    AuthenticationScheme = "PutANameHere",
            //    LoginPath = new PathString("/Account/Login/"),
            //    AutomaticAuthenticate = true,
            //    AutomaticChallenge = true
            //});
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    //template: "{controller=CreateSession}/{action=Index}/{id?}");
                    template: "{controller=Account}/{action=Login}/{id?}");
            });
        }
    }
}
