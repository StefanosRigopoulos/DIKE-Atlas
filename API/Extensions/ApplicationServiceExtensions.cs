using API.Data;
using API.Interfaces;
using API.Interfaces.Junctions;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddControllers()
                    .AddJsonOptions(options => options.JsonSerializerOptions.MaxDepth = 10);
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISearchRepository, SearchRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IPCRepository, PCRepository>();
            services.AddScoped<ICPURepository, CPURepository>();
            services.AddScoped<IMOBORepository, MOBORepository>();
            services.AddScoped<IRAMRepository, RAMRepository>();
            services.AddScoped<IGPURepository, GPURepository>();
            services.AddScoped<IPSURepository, PSURepository>();
            services.AddScoped<IStorageRepository, StorageRepository>();
            services.AddScoped<INetworkCardRepository, NetworkCardRepository>();
            services.AddScoped<IMonitorRepository, MonitorRepository>();
            services.AddScoped<IPCCPURepository, PCCPURepository>();
            services.AddScoped<IPCMOBORepository, PCMOBORepository>();
            services.AddScoped<IPCRAMRepository, PCRAMRepository>();
            services.AddScoped<IPCGPURepository, PCGPURepository>();
            services.AddScoped<IPCPSURepository, PCPSURepository>();
            services.AddScoped<IPCStorageRepository, PCStorageRepository>();
            services.AddScoped<IPCNetworkCardRepository, PCNetworkCardRepository>();
            services.AddScoped<IPCMonitorRepository, PCMonitorRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }
    }
}