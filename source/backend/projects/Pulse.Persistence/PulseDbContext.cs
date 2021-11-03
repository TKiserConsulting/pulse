namespace Pulse.Persistence
{
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using Data.Auth;
    using MassTransit;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Webfarm.Sdk.Common;
    using Webfarm.Sdk.Data;
    using Webfarm.Sdk.Data.Exceptions;
    using Webfarm.Sdk.Persistence.Configuration;
    using Webfarm.Sdk.Persistence.Extensions;

    public class PulseDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public const string Schema = "pulse";

        private readonly ILoggerFactory loggerFactory;
        private readonly IExecutionContext executionContext;
        private readonly ExceptionManager exceptionManager;

        public PulseDbContext(DbContextOptions<PulseDbContext> options)
            : base(options)
        {
            //this.auditContext = new DefaultAuditContext(this) { AuditDisabled = DisableAudit };
            //Helper.SetConfig(this.auditContext);
        }

        public PulseDbContext(
            DbContextOptions<PulseDbContext> options,
            ILoggerFactory loggerFactory,
            IExecutionContext executionContext = null,
            [Import(PersistenceCoreCompositionModule.DefaultExceptionManager)]
            ExceptionManager exceptionManager = null)
            : this(options)
        {
            this.loggerFactory = loggerFactory;
            this.executionContext = executionContext;
            this.exceptionManager = exceptionManager;
        }

        // profile
        public DbSet<InstructorProfile> InstructorProfiles { get; set; }

        // emoticons
        public DbSet<Emoticon> Emoticons { get; set; }

        public DbSet<InstructorEmoticon> InstructorEmoticons { get; set; }

        // identity
        public DbSet<IdentityRefreshToken> IdentityRefreshTokens { get; set; }

        // classes
        public DbSet<TheClass> Classes { get; set; }

        // sessions
        public DbSet<Session> Sessions { get; set; }

        public DbSet<SessionCheckin> SessionCheckins { get; set; }

        public DbSet<SessionQuestion> SessionQuestions { get; set; }

        // session students
        public DbSet<SessionStudent> SessionStudents { get; set; }

        public DbSet<EmoticonTap> EmoticonTaps { get; set; }

        public async Task<T> LoadAsync<T>(Guid id, CancellationToken cancellationToken) where T : class
        {
            return await this.LoadAsync<T, Guid>(id, cancellationToken);
        }

        public async Task<T> LoadAsync<T>(string id, CancellationToken cancellationToken) where T : class
        {
            return await this.LoadAsync<T, string>(id, cancellationToken);
        }

        public async Task<T> LoadAsync<T, TKey>(TKey id, CancellationToken cancellationToken) where T : class
        {
            var instance = await this.Set<T>().FindAsync(new object[] { id }, cancellationToken);
            if (instance == null)
            {
                throw new ObjectNotFoundException($"Object {typeof(T).Name}#{id} cannot be loaded.");
            }

            return instance;
        }

        public Task RemoveAsync<T>(Guid id, CancellationToken cancellationToken)
            where T : class, IIdentified<Guid>, new()
        {
            return this.RemoveAsync<T>(m => m.Id = id, cancellationToken);
        }

        public async Task RemoveAsync<T>([NotNull] Action<T> configure, CancellationToken cancellationToken)
            where T : class, new()
        {
            var instance = new T();
            configure(instance);
            var set = this.Set<T>();
            set.Attach(instance);
            set.Remove(instance);
            await this.SaveChangesAsync(cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (this.loggerFactory != null)
            {
                builder.UseLoggerFactory(this.loggerFactory);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.UseIdentityColumns();

            this.ConfigureMappings(modelBuilder);

            this.UseSingularTableNames(modelBuilder);

            this.UseSnakeCase(modelBuilder);
        }

        private bool HandleException(Exception ex)
        {
            var rethrow = true;
            if (this.exceptionManager != null)
            {
                rethrow = this.exceptionManager.HandleException(ex, PersistenceCoreCompositionModule.DefaultPolicy);
            }

            return rethrow;
        }

        private void ConfigureMappings(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<IdentityRefreshToken>()
                .HasIndex(m => m.Token)
                .IsUnique();

            //this.SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var userId = NewId.NextGuid();
            var adminRoleId = NewId.NextGuid();
            var instructorRoleId = NewId.NextGuid();
            var studentRoleId = NewId.NextGuid();

            modelBuilder.Entity<IdentityRole<Guid>>()
                .HasData(
                    new IdentityRole<Guid>
                    {
                        Id = adminRoleId,
                        Name = UserRole.Admin.ToString(),
                        NormalizedName = UserRole.Admin.ToString().ToUpper()
                    },
                    new IdentityRole<Guid>
                    {
                        Id = instructorRoleId,
                        Name = UserRole.Instructor.ToString(),
                        NormalizedName = UserRole.Instructor.ToString().ToUpper()
                    },
                    new IdentityRole<Guid>
                    {
                        Id = studentRoleId,
                        Name = UserRole.Student.ToString(),
                        NormalizedName = UserRole.Student.ToString().ToUpper()
                    }
                );

            modelBuilder.Entity<ApplicationUser>()
                .HasData(
                    new ApplicationUser
                    {
                        Id = userId,
                        Email = "admin@pulse.com",
                        UserName = "admin@pulse.com",
                        FirstName = "Super",
                        LastName = "Admin",
                        PasswordHash = ""
                    });

            modelBuilder.Entity<IdentityUserRole<Guid>>()
                .HasData(
                    new IdentityUserRole<Guid>
                    {
                        UserId = userId,
                        RoleId = adminRoleId
                    }
                );
        }
    }
}
