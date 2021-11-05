namespace Pulse.Api.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Common.Managers;
    using Data;
    using Data.Auth;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models.Profiles;
    using Persistence;
    using Webfarm.Sdk.Data.ComponentModel;
    using System.IO;
    using Microsoft.AspNetCore.Http;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;

    [GrantTypePrefix(WellKnownApplicationParts.Instructor)]
    [Route("[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly PulseDbContext db;
        private readonly IMapper mapper;
        private readonly AuthManager authManager;

        public ProfilesController(
            PulseDbContext db,
            IMapper mapper,
            AuthManager authManager)
        {
            this.db = db;
            this.mapper = mapper;
            this.authManager = authManager;
        }

        [HttpGet]
        public async Task<InstructorProfileDetailsDto> Load(CancellationToken cancellationToken)
        {
            var dto = await this.db.InstructorProfiles
                .Where(m => m.InstructorId == this.authManager.UserId)
                .AsNoTracking()
                .ProjectTo<InstructorProfileDetailsDto>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            dto ??= new InstructorProfileDetailsDto { Id = this.authManager.UserId };

            var user = await this.db.Set<ApplicationUser>()
                .AsNoTracking()
                .SingleAsync(m => m.Id == this.authManager.UserId, cancellationToken);

            dto.FirstName = user.FirstName;
            dto.LastName = user.LastName;

            return dto;
        }

        [HttpGet("image")]
        public async Task<IActionResult> LoadImage(CancellationToken cancellationToken)
        {
            var image = await this.db.InstructorProfiles
                .Where(m => m.InstructorId == this.authManager.UserId)
                .AsNoTracking()
                .Select(m => m.Image)
                .FirstOrDefaultAsync(cancellationToken);

            if (image != null)
            {
                return this.File(image, "image/jpeg");
            }

            return this.NoContent();
        }

        [HttpGet("smallimage")]
        public async Task<IActionResult> LoadSmallImage(CancellationToken cancellationToken)
        {
            var image = await this.db.InstructorProfiles
                .Where(m => m.InstructorId == this.authManager.UserId)
                .AsNoTracking()
                .Select(m => m.SmallImage)
                .FirstOrDefaultAsync(cancellationToken);

            if (image != null)
            {
                return this.File(image, "image/jpeg");
            }

            return this.NoContent();
        }

        [HttpPost]
        public async Task Update(InstructorProfileUpdateDto model, CancellationToken cancellationToken)
        {
            var instance = await this.db.InstructorProfiles
                .FirstOrDefaultAsync(m => m.InstructorId == this.authManager.UserId, cancellationToken);

            if (instance == null)
            {
                instance = this.mapper.Map<InstructorProfile>(model);
                instance.InstructorId = this.authManager.UserId;
                await this.db.AddAsync(instance, cancellationToken);
            }   
            else
            {
                this.mapper.Map(model, instance);
                this.db.Entry(instance).State = EntityState.Modified;
                this.db.Entry(instance).Property(m => m.Image).IsModified = false;
                this.db.Entry(instance).Property(m => m.SmallImage).IsModified = false;
            }

            var user = await this.db.Set<ApplicationUser>()
                .SingleAsync(m => m.Id == this.authManager.UserId, cancellationToken);

            if (user.FirstName != model.FirstName || user.LastName != model.LastName)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
            }

            await this.db.SaveChangesAsync(cancellationToken);
        }

        [HttpPost("image")]
        public async Task UploadImage([FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            var instance = await this.db.InstructorProfiles
                .FirstOrDefaultAsync(m => m.InstructorId == this.authManager.UserId, cancellationToken);

            var image = await GetBytes(file, cancellationToken);

            if (instance == null)
            {
                instance = new InstructorProfile
                {
                    InstructorId = this.authManager.UserId,
                    Image = await ResizeImage(image, 600, cancellationToken),
                    SmallImage = await ResizeImage(image, 100, cancellationToken)
                };
                await this.db.AddAsync(instance, cancellationToken);
            }
            else
            {
                instance.Image = await ResizeImage(image, 600, cancellationToken);
                instance.SmallImage = await ResizeImage(image, 100, cancellationToken);
                this.db.Entry(instance).State = EntityState.Modified;
            }

            await this.db.SaveChangesAsync(cancellationToken);
        }

        //[HttpPost("password")]
        //public async Task Password(UpdatePasswordDto model)
        //{
        //    var userId = this.executionContext.Principal.UserId();
        //    var user = await this.userManager.FindByIdAsync(userId);
        //    user.DemandUser();

        //    var checkPasswordResult = await this.signInManager.CheckPasswordSignInAsync(user, model.OldPassword, false);
        //    checkPasswordResult.DemandValid(nameof(model.OldPassword));

        //    var token = await this.userManager.GeneratePasswordResetTokenAsync(user);

        //    var newPasswordResult = await this.userManager.ResetPasswordAsync(user, token, model.NewPassword);
        //    newPasswordResult.DemandValid(nameof(model.NewPassword));
        //}

        private static async Task<byte[]> ResizeImage(byte[] original, int maxSize, CancellationToken cancellationToken)
        {
            using var image = Image.Load(original);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxSize, maxSize)
            }));
            await using var imageStream = new MemoryStream();
            await image.SaveAsJpegAsync(imageStream, cancellationToken);
            return imageStream.ToArray();
        }

        private static async Task<byte[]> GetBytes(IFormFile photo, CancellationToken cancellationToken)
        {
            await using var memoryStream = new MemoryStream();
            await photo.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();

            return fileBytes;
        }
    }
}
