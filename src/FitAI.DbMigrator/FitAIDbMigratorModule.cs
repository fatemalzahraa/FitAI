using FitAI.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace FitAI.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(FitAIEntityFrameworkCoreModule),
    typeof(FitAIApplicationContractsModule)
    )]
public class FitAIDbMigratorModule : AbpModule
{
}
