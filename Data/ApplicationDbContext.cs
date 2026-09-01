using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Models;

namespace TaskManagement.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Team> Teams => Set<Team>();

        public DbSet<TeamMember> TeamMembers => Set<TeamMember>();

        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        public DbSet<Comment> Comments => Set<Comment>();

        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Team -> Manager
            modelBuilder.Entity<Team>()
                .HasOne(x => x.Manager)
                .WithMany()
                .HasForeignKey(x => x.ManagerId)
                .OnDelete(DeleteBehavior.SetNull);


            // TeamMember Composite Key
            modelBuilder.Entity<TeamMember>()
                .HasKey(x => new { x.TeamId, x.UserId });

            // Team -> TeamMembers
            modelBuilder.Entity<TeamMember>()
                .HasOne(x => x.Team)
                .WithMany(x => x.Members)
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> TeamMembers
            modelBuilder.Entity<TeamMember>()
                .HasOne(x => x.User)
                .WithMany(x => x.TeamMemberships)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Team -> Tasks
            modelBuilder.Entity<TaskItem>()
                .HasOne(x => x.Team)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assigned User -> Tasks
            modelBuilder.Entity<TaskItem>()
                .HasOne(x => x.AssignedToUser)
                .WithMany(x => x.AssignedTasks)
                .HasForeignKey(x => x.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Created User -> Tasks
            modelBuilder.Entity<TaskItem>()
                .HasOne(x => x.CreatedByUser)
                .WithMany(x => x.CreatedTasks)
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Task -> Comments
            modelBuilder.Entity<Comment>()
                .HasOne(x => x.TaskItem)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> Comments
            modelBuilder.Entity<Comment>()
                .HasOne(x => x.User)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> Notifications
            modelBuilder.Entity<Notification>()
                .HasOne(x => x.User)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Task -> Notifications
            modelBuilder.Entity<Notification>()
                .HasOne(x => x.TaskItem)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.TaskItemId)
                .OnDelete(DeleteBehavior.SetNull);

            // Unique email
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();
        }
    }
}