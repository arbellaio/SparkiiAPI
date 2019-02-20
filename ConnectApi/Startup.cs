using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ConnectApi.AppProperties;
using ConnectApi.Services.AppDatabaseContext;
using ConnectApi.Services.Users;
using Hangfire;
using Hangfire.Common;
using Hangfire.SqlServer;
using Hangfire.States;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace ConnectApi
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
             services.AddDbContext<AppDataContext>(x =>
                x.UseSqlServer(Configuration.GetConnectionString("DataAccessSqlServerProvider")));
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DataAccessSqlServerProvider")));
            services.Configure<IISOptions>(options => { options.ForwardClientCertificate = false; });

            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Info{Title = "ConnectAPI", Description = "Swagger Documentation"}));

            JobStorage.Current = new SqlServerStorage(Configuration.GetConnectionString("DataAccessSqlServerProvider"));
            RecurringJob.AddOrUpdate<IUserService>(x => x.ChangeStatusForUsersWithActiveTime(), Cron.Minutely);



            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.AddCors();
            var appSettingsSection = Configuration.GetSection("AppSettings");

            services.Configure<AppConstants>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppConstants>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
                {
                    x.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    x.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    x.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options => { options.LoginPath = "/"; })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, x =>
                {
                    x.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                            var username = context.Principal.Identity.Name;
                            var user = userService.GetUserByEmail(username);
                            if (user == null)
                            {
                                // return unauthorized if user no longer exists
                                context.Fail("Unauthorized");
                            }

                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized) {ReasonPhrase = "Oops!!!"};
                            return Task.CompletedTask;
                        }
                    };
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;

                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ValidateAudience = false
                    };
                });

            services.AddScoped<IUserService, UserService>();
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
                app.UseHsts();
            }
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            app.UseAuthentication();
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Core API"));
            app.UseMvc();
        }

//        public void RunBackGroundJobStatusCheck()
//        {
//            Task.Run(async () =>
//            {
//               await _service.ChangeStatusForUsersWithActiveTime();
//            });
//        }
    }
}