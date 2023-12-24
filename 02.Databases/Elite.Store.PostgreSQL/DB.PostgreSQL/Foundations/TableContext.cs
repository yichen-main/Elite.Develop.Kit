namespace PostgreSQL.Foundations;

[Dependency(ServiceLifetime.Scoped)]
internal sealed class TableContext(DbContextOptions<TableContext> options) : DbContext(options)
{
    public DbSet<AtomUserInfo> AtomUserInfo { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AtomBookInfo>()
            .HasOne(item => item.User)
            .WithMany(item => item.Books)
            .HasForeignKey(item => item.UserId);
    }
}