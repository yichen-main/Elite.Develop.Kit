namespace PostgreSQL.Foundations.Demos.Users;

[Table("demo_user_info")]
internal sealed class AtomUserInfo
{
    [Key, Column("id")] public required Guid Id { get; init; }
    [Column("pole_type")] public required RoleType RoleType { get; set; }
    [Column("user_name")] public required string UserName { get; set; }
    [Column("create_time")] public required DateTime CreateTime { get; init; }

    //參考導覽屬性
    public ICollection<AtomBookInfo> Books { get; init; } = [];
}
internal enum RoleType
{
    Administrator,
    Manager,
    Operator
}