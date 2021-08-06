using HotChocolate;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types.Relay;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace hotchocolate_playground
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
            services.AddPooledDbContextFactory<ApplicationDbContext>(options => options.UseSqlite("Data Source=test.db"));

            services
                .AddGraphQLServer()
                .AddQueryType(d => d.Name("Query"))
                .AddTypeExtension<ExampleQueries>()
                .AddIdSerializer(s => new CustomIdSerializer())
                .ConfigureTypes()
                .EnableRelaySupport()
                .AddDataLoader<ExampleByIdDataLoader>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
            });
        }
    }

    public class CustomIdSerializer : IIdSerializer
    {
        public string Serialize<T>(NameString schemaName, NameString typeName, T id)
        {
            return JsonConvert.SerializeObject(id, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }

        public IdValue Deserialize(string serializedId)
        {
            var result = JsonConvert.DeserializeObject(serializedId, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            return new IdValue(null, nameof(CompositeKey), result);
        }
    }

    public static class TypesConfiguration
    {
        public static IRequestExecutorBuilder ConfigureTypes(this IRequestExecutorBuilder builder)
            => builder.AddType<ExampleType>();
    }
}
