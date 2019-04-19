using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ML;
using PostMatch.Api.Core;
using PostMatch.Api.Helpers;
using PostMatch.Api.Models;
using PostMatch.Core.Interface;
using PostMatch.Infrastructure.DataAccess.Implement;
using PostMatch.Infrastructure.DataAccess.Interface;
using PostMatch.Infrastructure.DataBase;
using PostMatch.Infrastructure.Services;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PostMatch.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(new ExceptionAttribute());
            });
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

            services.AddHttpsRedirection(options =>
           {
               options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
               options.HttpsPort = 5000;
           });

            services.AddAutoMapper();

            //注册数据库连接
            services.AddDbContext<MyContext>(options => 
            options.UseMySQL(_configuration.GetConnectionString("DefaultConnection")));

            //注册数据库操作类
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<ICompanyRepository, CompanyUserRepository>();
            services.AddScoped<IAdministratorService, AdministratorService>();
            services.AddScoped<IAdministratorRepository, AdministratorRepository>();
            services.AddScoped<IResumeService, ResumeService>();
            services.AddScoped<IResumeRepository, ResumeRepository>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddScoped<IDeliveryRepository, DeliveryRepository>();
            services.AddScoped<IRecommendService, RecommendService>();
            services.AddScoped<IRecommendRepository, RecommendRepository>();
            services.AddScoped<IInterviewService, InterviewService>();
            services.AddScoped<IInterviewRepository, InterviewRepository>();

            // 配置强类型设置对象
            var appSettingsSection = _configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // 配置jwt身份验证
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                            var userId = context.Principal.Identity.Name;
                            var user = userService.GetById(userId);
                            if (user == null)
                            {
                                // return unauthorized if user no longer exists
                                context.Fail("Unauthorized");
                            }
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
                        ValidateAudience = false
                    };
                });

            //配置跨域
            services.AddCors(options =>
            {
                // 添加一个策略，AllowAngularDevOrigin为自定义名字，
                //  builder.WithOrigins("http://localhost:4200")把所有从这个地址发出来的请求都允许
                options.AddPolicy("AllowAngularDevOrigin",
                    builder => builder.WithOrigins("http://localhost:4200")
                        .WithExposedHeaders("X-Pagination")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

            //添加PredictionEngine
            services.AddScoped<MLContext>();
            services.AddScoped<PredictionEngine<Resume, ResumePrediction>>((ctx) =>
            {
                MLContext mlContext = ctx.GetRequiredService<MLContext>();
                string modelFilePathName = "Models/model.zip";

                //Load model from file
                ITransformer model;
                using (var stream = File.OpenRead(modelFilePathName))
                {
                    model = mlContext.Model.Load(stream);
                }

                // Return prediction engine
                return model.CreatePredictionEngine<Resume, ResumePrediction>(mlContext);
            });

            //添加PredictionEngine
            services.AddScoped<MLContext>();
            services.AddScoped<PredictionEngine<PostMatching, PostMatchingPrediction>>((ctx) =>
            {
                MLContext mlContext = ctx.GetRequiredService<MLContext>();
                string modelFilePathName = "Models/postModel.zip";

                //Load model from file
                ITransformer model;
                using (var stream = File.OpenRead(modelFilePathName))
                {
                    model = mlContext.Model.Load(stream);
                }

                // Return prediction engine
                return model.CreatePredictionEngine<PostMatching, PostMatchingPrediction>(mlContext);
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
                app.UseHsts();
            }

            // 配置跨域
            app.UseCors("AllowAngularDevOrigin");

            app.Use(async (context, next) =>
            {
                if (!context.Request.Path.ToString().StartsWith("/api/passport"))
                {
                    var _token = "";
                    //var _name = "";
                    //var _id = "";
                    if (context.Request.Headers.TryGetValue("token", out var tokens) && tokens.Count > 0)
                    {
                        _token = tokens[0];
                        //if(context.Request.Headers.TryGetValue("User", out var users) && tokens.Count > 0)
                        //{
                        //    string[] sArray = Regex.Split(users, ",", RegexOptions.IgnoreCase);
                        //    _name = sArray[0];
                        //    _id = sArray[1];
                        //}

                    }
                    if (_token == "")
                    {
                        context.Response.StatusCode = 401;
                        return;
                    }

                    //var user = new UserModel
                    //{
                    //    Id = _id,
                    //    Name = _name
                    //};
                    context.Items.Add("token", _token);
                    //context.Items.Add("user", user);
                }
                await next();
            });
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseMvc();
        }
    }
}
