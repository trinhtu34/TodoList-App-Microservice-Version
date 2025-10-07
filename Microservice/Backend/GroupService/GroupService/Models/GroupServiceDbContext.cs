using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace GroupService.Models;

public partial class GroupServiceDbContext : DbContext
{
    public GroupServiceDbContext()
    {
    }

    public GroupServiceDbContext(DbContextOptions<GroupServiceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DirectMessageGroup> DirectMessageGroups { get; set; }

    public virtual DbSet<GroupInvitation> GroupInvitations { get; set; }

    public virtual DbSet<GroupMember> GroupMembers { get; set; }

    public virtual DbSet<GroupsR> GroupsRs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<DirectMessageGroup>(entity =>
        {
            entity.HasKey(e => new { e.User1Id, e.User2Id })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("direct_message_groups", tb => tb.HasComment("Quick lookup for existing 1-1 conversations between two users"));

            entity.HasIndex(e => e.GroupId, "idx_group").IsUnique();

            entity.Property(e => e.User1Id)
                .HasMaxLength(50)
                .HasComment("Smaller cognito_sub (alphabetically)")
                .HasColumnName("user1_id");
            entity.Property(e => e.User2Id)
                .HasMaxLength(50)
                .HasComment("Larger cognito_sub (alphabetically)")
                .HasColumnName("user2_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GroupId).HasColumnName("group_id");

            entity.HasOne(d => d.Group).WithOne(p => p.DirectMessageGroup)
                .HasForeignKey<DirectMessageGroup>(d => d.GroupId)
                .HasConstraintName("direct_message_groups_ibfk_1");
        });

        modelBuilder.Entity<GroupInvitation>(entity =>
        {
            entity.HasKey(e => e.InvitationId).HasName("PRIMARY");

            entity.ToTable("group_invitations", tb => tb.HasComment("Pending group invitations"));

            entity.HasIndex(e => e.GroupId, "idx_group");

            entity.HasIndex(e => new { e.InvitedUser, e.Status }, "idx_invited_user");

            entity.HasIndex(e => e.Status, "idx_status");

            entity.Property(e => e.InvitationId).HasColumnName("invitation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("datetime")
                .HasColumnName("expires_at");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.InvitedBy)
                .HasMaxLength(50)
                .HasComment("cognito_sub")
                .HasColumnName("invited_by");
            entity.Property(e => e.InvitedUser)
                .HasMaxLength(50)
                .HasComment("cognito_sub or email")
                .HasColumnName("invited_user");
            entity.Property(e => e.RespondedAt)
                .HasColumnType("datetime")
                .HasColumnName("responded_at");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','accepted','declined','expired')")
                .HasColumnName("status");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupInvitations)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("group_invitations_ibfk_1");
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.UserId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("group_members", tb => tb.HasComment("Members in groups and 1-1 conversations"));

            entity.HasIndex(e => e.Role, "idx_role");

            entity.HasIndex(e => e.UserId, "idx_user_id");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .HasComment("cognito_sub")
                .HasColumnName("user_id");
            entity.Property(e => e.IsMuted)
                .HasDefaultValueSql("'0'")
                .HasComment("User muted notifications")
                .HasColumnName("is_muted");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("joined_at");
            entity.Property(e => e.LastReadAt)
                .HasComment("Last time user read messages in this group")
                .HasColumnType("datetime")
                .HasColumnName("last_read_at");
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'member'")
                .HasColumnType("enum('owner','admin','member')")
                .HasColumnName("role");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("group_members_ibfk_1");
        });

        modelBuilder.Entity<GroupsR>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PRIMARY");

            entity.ToTable("groups_r", tb => tb.HasComment("Stores both 1-1 conversations and group chats"));

            entity.HasIndex(e => e.IsActive, "idx_active");

            entity.HasIndex(e => e.CreatedBy, "idx_created_by");

            entity.HasIndex(e => e.GroupType, "idx_group_type");

            entity.HasIndex(e => e.LastMessageAt, "idx_last_message").IsDescending();

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasComment("cognito_sub")
                .HasColumnName("created_by");
            entity.Property(e => e.GroupAvatar)
                .HasMaxLength(500)
                .HasColumnName("group_avatar");
            entity.Property(e => e.GroupName)
                .HasMaxLength(255)
                .HasComment("NULL for 1-1 chats or unnamed groups")
                .HasColumnName("group_name");
            entity.Property(e => e.GroupType)
                .HasDefaultValueSql("'group'")
                .HasComment("direct=1-1 chat, group=multiple users")
                .HasColumnType("enum('direct','group')")
                .HasColumnName("group_type");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.LastMessageAt)
                .HasComment("For sorting by recent activity")
                .HasColumnType("datetime")
                .HasColumnName("last_message_at");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
