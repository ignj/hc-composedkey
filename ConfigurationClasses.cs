using GreenDonut;
using HotChocolate;
using HotChocolate.DataLoader;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace hotchocolate_playground
{

    /// HOT CHOCOLATE CONFIGURATION

    [ExtendObjectType("Query")]
    public class ExampleQueries
    {
        [UseApplicationDbContext]
        public IQueryable<Example> GetExamples([ScopedService] ApplicationDbContext context) => context.Examples;
    }

    public class ExampleType : ObjectType<Example>
    {
        protected override void Configure(IObjectTypeDescriptor<Example> descriptor)
        {
            descriptor
                .ImplementsNode()
                .IdField(t => t.ComposedKey)
                .ResolveNode((ctx, id) => ctx.DataLoader<ExampleByIdDataLoader>().LoadAsync(id, ctx.RequestAborted));
        }
    }

    public class ExampleByIdDataLoader : BatchDataLoader<string, Example>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public ExampleByIdDataLoader(
            IBatchScheduler batchScheduler,
            IDbContextFactory<ApplicationDbContext> dbContextFactory)
            : base(batchScheduler)
        {
            _dbContextFactory = dbContextFactory ??
                throw new ArgumentNullException(nameof(dbContextFactory));
        }

        protected override async Task<IReadOnlyDictionary<string, Example>> LoadBatchAsync(
            IReadOnlyList<string> keysString,
            CancellationToken cancellationToken)
        {
            await using ApplicationDbContext dbContext =
                _dbContextFactory.CreateDbContext();

            List<CompositeKey> keys = new List<CompositeKey>();
            foreach (var key in keysString) keys.Add(JsonSerializer.Deserialize<CompositeKey>(key));

            IQueryable<Example> query = null;
            foreach (var ck in keys)
            {
                var temp = dbContext.Examples.Where(x => x.Id == ck.Id && x.TypeId == ck.TypeId);
                query = query == null ? temp : query.Union(temp);
            }
            return await query.ToDictionaryAsync(t => t.ComposedKey, cancellationToken);
        }
    }

    public class UseApplicationDbContextAttribute : ObjectFieldDescriptorAttribute
    {
        public override void OnConfigure(
            IDescriptorContext context,
            IObjectFieldDescriptor descriptor,
            MemberInfo member) => descriptor.UseDbContext<ApplicationDbContext>();
    }
}