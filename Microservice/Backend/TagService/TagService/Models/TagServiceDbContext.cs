using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace TagService.Models;

public partial class TagServiceDbContext : DbContext
{
    public TagServiceDbContext()
    {
    }

    public TagServiceDbContext(DbContextOptions<TagServiceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TodoTag> TodoTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PRIMARY");

            entity.ToTable("tags");

            entity.HasIndex(e => e.CognitoSub, "idx_cognito_sub");

            entity.HasIndex(e => e.GroupId, "idx_group_id");

            entity.HasIndex(e => new { e.TagName, e.CognitoSub, e.GroupId }, "unique_tag_per_scope").IsUnique();

            entity.Property(e => e.TagId).HasColumnName("tag_id");
            entity.Property(e => e.CognitoSub)
                .HasMaxLength(50)
                .HasComment("Creator")
                .HasColumnName("cognito_sub");
            entity.Property(e => e.Color)
                .HasMaxLength(7)
                .HasDefaultValueSql("'#808080'")
                .HasComment("Hex color")
                .HasColumnName("color");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GroupId)
                .HasComment("NULL = personal tag, NOT NULL = group tag")
                .HasColumnName("group_id");
            entity.Property(e => e.TagName)
                .HasMaxLength(50)
                .HasColumnName("tag_name");
        });

        modelBuilder.Entity<TodoTag>(entity =>
        {
            entity.HasKey(e => new { e.TodoId, e.TagId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("todo_tags");

            entity.HasIndex(e => e.TagId, "idx_tag_id");

            entity.HasIndex(e => e.TodoId, "idx_todo_id");

            entity.Property(e => e.TodoId).HasColumnName("todo_id");
            entity.Property(e => e.TagId).HasColumnName("tag_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Tag).WithMany(p => p.TodoTags)
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("todo_tags_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
