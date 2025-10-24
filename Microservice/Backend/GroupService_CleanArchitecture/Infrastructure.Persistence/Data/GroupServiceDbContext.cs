using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Data;

public class GroupServiceDbContext : DbContext
{
    public GroupServiceDbContext(DbContextOptions<GroupServiceDbContext> options) : base(options)
    {
    }

    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<GroupInvitation> GroupInvitations { get; set; }
    public DbSet<DirectMessageGroup> DirectMessageGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        // Group configuration
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId);
            entity.ToTable("groups_r");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.GroupName).HasMaxLength(255).HasColumnName("group_name");
            entity.Property(e => e.GroupAvatar).HasMaxLength(500).HasColumnName("group_avatar");
            entity.Property(e => e.GroupDescription).HasColumnType("text").HasColumnName("group_description");
            entity.Property(e => e.GroupType)
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<GroupType>(v, true))
                .HasColumnName("group_type");
            entity.Property(e => e.CreatedBy).HasMaxLength(50).HasColumnName("created_by");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime").HasColumnName("updated_at");
            entity.Property(e => e.LastMessageAt).HasColumnType("datetime").HasColumnName("last_message_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active");

            entity.HasIndex(e => e.IsActive).HasDatabaseName("idx_active");
            entity.HasIndex(e => e.CreatedBy).HasDatabaseName("idx_created_by");
            entity.HasIndex(e => e.GroupType).HasDatabaseName("idx_group_type");
        });

        // GroupMember configuration
        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.UserId });
            entity.ToTable("group_members");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.UserId).HasMaxLength(50).HasColumnName("user_id");
            entity.Property(e => e.Role)
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<MemberRole>(v, true))
                .HasColumnName("role");
            entity.Property(e => e.Nickname).HasMaxLength(100).HasColumnName("nickname");
            entity.Property(e => e.JoinedAt).HasColumnType("datetime").HasColumnName("joined_at");
            entity.Property(e => e.LastReadAt).HasColumnType("datetime").HasColumnName("last_read_at");
            entity.Property(e => e.IsMuted).HasColumnName("is_muted");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.LeftAt).HasColumnType("datetime").HasColumnName("left_at");

            entity.HasOne(d => d.Group)
                .WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.GroupId);

            entity.HasIndex(e => e.UserId).HasDatabaseName("idx_user_id");
            entity.HasIndex(e => e.Role).HasDatabaseName("idx_role");
        });

        // GroupInvitation configuration
        modelBuilder.Entity<GroupInvitation>(entity =>
        {
            entity.HasKey(e => e.InvitationId);
            entity.ToTable("group_invitations");

            entity.Property(e => e.InvitationId).HasColumnName("invitation_id");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.InvitedBy).HasMaxLength(50).HasColumnName("invited_by");
            entity.Property(e => e.InvitedUser).HasMaxLength(50).HasColumnName("invited_user");
            entity.Property(e => e.Status)
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<InvitationStatus>(v, true))
                .HasColumnName("status");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.RespondedAt).HasColumnType("datetime").HasColumnName("responded_at");
            entity.Property(e => e.ExpiresAt).HasColumnType("datetime").HasColumnName("expires_at");

            entity.HasOne(d => d.Group)
                .WithMany(p => p.GroupInvitations)
                .HasForeignKey(d => d.GroupId);

            entity.HasIndex(e => e.GroupId).HasDatabaseName("idx_group");
            entity.HasIndex(e => e.Status).HasDatabaseName("idx_status");
        });

        // DirectMessageGroup configuration
        modelBuilder.Entity<DirectMessageGroup>(entity =>
        {
            entity.HasKey(e => new { e.User1Id, e.User2Id });
            entity.ToTable("direct_message_groups");

            entity.Property(e => e.User1Id).HasMaxLength(50).HasColumnName("user1_id");
            entity.Property(e => e.User2Id).HasMaxLength(50).HasColumnName("user2_id");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasColumnName("created_at");

            entity.HasOne(d => d.Group)
                .WithOne(p => p.DirectMessageGroup)
                .HasForeignKey<DirectMessageGroup>(d => d.GroupId);

            entity.HasIndex(e => e.GroupId).HasDatabaseName("idx_group").IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
