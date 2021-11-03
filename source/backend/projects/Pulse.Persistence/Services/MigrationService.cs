namespace Pulse.Persistence.Services
{
    using System;
    using System.Drawing;
    using System.Threading;
    using Data;
    using Data.Auth;
    using ImTools;
    using MassTransit;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Webfarm.Sdk.Persistence.Migrations;

    public sealed class MigrationService : CoreMigratorHostedService
    {
        public MigrationService(
            IServiceProvider serviceProvider,
            ILogger logger)
            :base(serviceProvider, logger)
        {
        }

        protected override void Migrate(IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            var creator = new DatabaseCreator(serviceProvider);

            creator.CreateSchema(out var newSchema);

            if (newSchema)
            {
                this.SeedData(serviceProvider);
            }

            stoppingToken.ThrowIfCancellationRequested();

            // base.Migrate(serviceProvider, stoppingToken);
        }

        private void SeedData(IServiceProvider serviceProvider)
        {
            var adminId = NewId.NextGuid();
            var instructorId = NewId.NextGuid();
            var adminRoleId = NewId.NextGuid();
            var instructorRoleId = NewId.NextGuid();
            var studentRoleId = NewId.NextGuid();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var db = serviceProvider.GetRequiredService<PulseDbContext>();

            // roles
            roleManager.CreateAsync(new IdentityRole<Guid>
            {
                Id = adminRoleId,
                Name = UserRole.Admin.ToString(),
                NormalizedName = UserRole.Admin.ToString().ToUpper()
            }).Wait();

            roleManager.CreateAsync(new IdentityRole<Guid>
            {
                Id = instructorRoleId,
                Name = UserRole.Instructor.ToString(),
                NormalizedName = UserRole.Instructor.ToString().ToUpper()
            }).Wait();

            roleManager.CreateAsync(new IdentityRole<Guid>
            {
                Id = studentRoleId,
                Name = UserRole.Student.ToString(),
                NormalizedName = UserRole.Student.ToString().ToUpper()
            }).Wait();

            // users
            var user = new ApplicationUser
            {
                Id = adminId,
                Email = "admin@pulse.com",
                UserName = "admin@pulse.com",
                FirstName = "Super",
                LastName = "Admin"
            };

            userManager.CreateAsync(user, "p$lse").Wait();

            userManager.AddToRoleAsync(user, UserRole.Admin.ToString()).Wait();

            user = new ApplicationUser
            {
                Id = instructorId,
                Email = "instructor@pulse.com",
                UserName = "instructor@pulse.com",
                FirstName = "Instructor",
                LastName = "#1"
            };

            userManager.CreateAsync(user, "pulse").Wait();

            userManager.AddToRoleAsync(user, UserRole.Instructor.ToString()).Wait();

            // emoticons
            var emoticons = new[]
            {
                new Emoticon { Id = NewId.NextGuid(), Title = "I understand", Color = ColorToHex(Color.Green), SortIndex = 1 },
                new Emoticon { Id = NewId.NextGuid(), Title = "I don't understand", Color = ColorToHex(Color.DarkRed), SortIndex = 2 },
                new Emoticon { Id = NewId.NextGuid(), Title = "Repeat, please", Color = ColorToHex(Color.Yellow), SortIndex = 3 },
                new Emoticon { Id = NewId.NextGuid(), Title = "Clarify, please", Color = ColorToHex(Color.CornflowerBlue), SortIndex = 4 },
                new Emoticon { Id = NewId.NextGuid(), Title = "Slow down", Color = ColorToHex(Color.DarkViolet), SortIndex = 5 },
                new Emoticon { Id = NewId.NextGuid(), Title = "Aha!", Color = ColorToHex(Color.DarkOliveGreen), SortIndex = 6 },
            };
            db.Emoticons.AddRange(emoticons);

            var instructorEmoticons = emoticons.Map(e => new InstructorEmoticon
            {
                Id = NewId.NextGuid(),
                Title = e.Title,
                Color = e.Color,
                SortIndex = e.SortIndex,
                InstructorId = instructorId
            });
            db.InstructorEmoticons.AddRange(instructorEmoticons);

            // classes
            db.Classes.Add(new TheClass { InstructorId = instructorId, Name = "Very First Class" });
            db.SaveChanges();
        }

        private static string ColorToHex(Color color)
        {
            #pragma warning disable CA1305 // Specify IFormatProvider
            // return color.ToArgb().ToString("x6");
            return ColorTranslator.ToHtml(Color.FromArgb(color.ToArgb()));
            #pragma warning restore CA1305 // Specify IFormatProvider
        }
    }
}
