namespace PostgreSQL.Foundations.Demos.Users;

[Table("demo_book_info")]
internal sealed class AtomBookInfo
{
    [Key, Column("id")] public required Guid Id { get; init; }
    [ForeignKey(nameof(AtomUserInfo)), Column("user_id")] public Guid UserId { get; init; }
    [Column("content")] public required string Content { get; set; }
    [Column("create_time")] public required DateTime CreateTime { get; init; }

    //反向導覽屬性
    public AtomUserInfo? User { get; set; }
}