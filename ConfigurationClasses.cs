using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using HotChocolate;
using HotChocolate.DataLoader;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using Microsoft.EntityFrameworkCore;

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

    public class ExampleByIdDataLoader : BatchDataLoader<CompositeKey, Example>
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

        protected override async Task<IReadOnlyDictionary<CompositeKey, Example>> LoadBatchAsync(
            IReadOnlyList<CompositeKey> keys,
            CancellationToken cancellationToken)
        {
            await using ApplicationDbContext dbContext =
                _dbContextFactory.CreateDbContext();

            return await dbContext.Examples
                .Where(s => keys.Any(k => k.Id == s.Id && k.TypeId == s.TypeId))
                .ToDictionaryAsync(t => t.ComposedKey, cancellationToken);
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