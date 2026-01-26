using Microsoft.EntityFrameworkCore;
using Database.Entities;

namespace Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options: options)
{
    public DbSet<TradeEntity> Trades { get; set; }

    public DbSet<TradeInfoEntity> TradeInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TradeEntity>(tradeEntity =>
        {
            tradeEntity.HasKey(trade => trade.Id);

            tradeEntity.Property(trade => trade.Name);

            tradeEntity
                .HasOne(trade => trade.TradeInfo)
                .WithOne(tradeInfo => tradeInfo.Trade)
                .HasForeignKey<TradeInfoEntity>(tradeInfo => tradeInfo.TradeId);
        });

        modelBuilder.Entity<TradeInfoEntity>(tradeInfoEntity =>
        {
            tradeInfoEntity.HasKey(tradeInfo => tradeInfo.TradeId);
        });
    }
}
