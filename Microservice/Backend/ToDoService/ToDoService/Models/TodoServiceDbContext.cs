using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace ToDoService.Models;

public partial class TodoServiceDbContext : DbContext
{
    public TodoServiceDbContext()
    {
    }

    public TodoServiceDbContext(DbContextOptions<TodoServiceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Todo> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Todo>(entity =>
        {
            entity.HasKey(e => e.TodoId).HasName("PRIMARY");

            entity.ToTable("todos");

            entity.HasIndex(e => e.AssignedTo, "idx_assigned_to");

            entity.HasIndex(e => e.CognitoSub, "idx_cognito_sub");

            entity.HasIndex(e => e.DueDate, "idx_due_date");

            entity.HasIndex(e => e.GroupId, "idx_group_id");

            entity.HasIndex(e => e.IsDone, "idx_is_done");

            entity.Property(e => e.TodoId).HasColumnName("todo_id");
            entity.Property(e => e.AssignedTo)
                .HasMaxLength(50)
                .HasComment("NULL = unassigned, cognito_sub of assignee")
                .HasColumnName("assigned_to");
            entity.Property(e => e.CognitoSub)
                .HasMaxLength(50)
                .HasComment("Owner/Creator")
                .HasColumnName("cognito_sub");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.DueDate)
                .HasColumnType("datetime")
                .HasColumnName("due_date");
            entity.Property(e => e.GroupId)
                .HasComment("NULL = personal todo, NOT NULL = group todo")
                .HasColumnName("group_id");
            entity.Property(e => e.IsDone)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_done");
            entity.Property(e => e.UpdateAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("update_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
