using Microsoft.EntityFrameworkCore;
using Database.Entities;

namespace Database;

/// <summary>
/// An DbContext implementation for the management of trade concerns.
/// </summary>
/// <param name="options">Options for configuring the initialized context.</param>
public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options: options)
{
    /// <summary>
    /// Defines a Trades table.
    /// </summary>
    public DbSet<TradeEntity> Trades { get; set; }

    /// <summary>
    /// Defines a TradeInfos table.
    /// </summary>
    public DbSet<TradeInfoEntity> TradeInfos { get; set; }

    /// <summary>
    /// Modify the schema of the database on entities that are being created.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance used to modify the schema.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /// 1. Set the primary key for the Trades table.
        /// 2. Add a 1-to-1 relationship between Trades and TradeInfos.
        modelBuilder.Entity<TradeEntity>(tradeEntity =>
        {
            tradeEntity.HasKey(trade => trade.Id);

            tradeEntity.Property(trade => trade.Name);

            tradeEntity
                .HasOne(trade => trade.TradeInfo)
                .WithOne(tradeInfo => tradeInfo.Trade)
                .HasForeignKey<TradeInfoEntity>(tradeInfo => tradeInfo.TradeId);
        });

        // Set the primary key for the TradeInfos table.
        modelBuilder.Entity<TradeInfoEntity>(tradeInfoEntity =>
        {
            tradeInfoEntity.HasKey(tradeInfo => tradeInfo.TradeId);
        });
    }
}
