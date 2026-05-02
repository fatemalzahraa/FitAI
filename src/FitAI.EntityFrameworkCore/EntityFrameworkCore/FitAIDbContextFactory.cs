using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FitAI.EntityFrameworkCore;

public class FitAIDbContextFactory : IDesignTimeDbContextFactory<FitAIDbContext>
{
    public FitAIDbContext CreateDbContext(string[] args)
    {
        FitAIEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<FitAIDbContext>();

        // SQL SERVER (ABP + EF Core uyumlu)
        builder.UseSqlServer(
            "Server=AYSENUR;Database=FitAI;Trusted_Connection=True;TrustServerCertificate=True"
        );

        return new FitAIDbContext(builder.Options);
    }
}