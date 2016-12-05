using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using Microsoft.Net.Http.Headers;
using SignalRChat.Domain;

namespace SignalRChat.Controllers
{
    public class ChatController : Controller
    {
	    private readonly IHostingEnvironment _environment;
	    private readonly IOptions<CommonSettings> _settings;

	    public ChatController(IHostingEnvironment environment, IOptions<CommonSettings> settings)
	    {
		    _environment = environment;
		    _settings = settings;
	    }

	    public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

	    public FileContentResult Avatar(string name)
	    {
			var avatarFile = Path.Combine(GetAvatarsFolderPath(), name);
		    if (!System.IO.File.Exists(avatarFile))
		    {
			    avatarFile = Path.Combine(_environment.WebRootPath, "static", "noAvatar.jpg");
		    }

		    var content = System.IO.File.ReadAllBytes(avatarFile);
			return new FileContentResult(content, "image/jpeg");
		}

		public FileContentResult Image(string imgName)
		{
			var imgFile = Path.Combine(GetUploadsFolderPath(), imgName);
			var content = System.IO.File.ReadAllBytes(imgFile);
			return new FileContentResult(content, "image/jpeg");
		}

		public FileContentResult Audio(string audio)
		{
			var audioFile = Path.Combine(GetUploadsFolderPath(), audio);
			var content = System.IO.File.ReadAllBytes(audioFile);
			return new FileContentResult(content, "audio/mpeg");
		}

		public FileContentResult Video(string video)
		{
			var videoFile = Path.Combine(GetUploadsFolderPath(), video);
			var content = System.IO.File.ReadAllBytes(videoFile);
			return new FileContentResult(content, "video/mp4");
		}

		[HttpPost]
		public ActionResult AddAvatar(string name, IFormFile theFile)
		{
			if (theFile == null || theFile.Length <= 0)
				return HttpBadRequest();
				
			var filePath = Path.Combine(GetAvatarsFolderPath(), name);
			theFile.SaveAs(filePath);

			return new JsonResult("ok");
		}

		[HttpPost]
		public ActionResult AddImage(IFormFile theFile)
		{
			if (theFile == null || theFile.Length <= 0)
				return HttpBadRequest();

			var uploads = GetUploadsFolderPath();

			var fileName = ContentDispositionHeaderValue.Parse(theFile.ContentDisposition).FileName.Trim('"');
			var filePath = Path.Combine(uploads, fileName);

			using (var stream = theFile.OpenReadStream())
			{
				var image = System.Drawing.Image.FromStream(stream);

				if (image.Width > _settings.Options.MaxUploadImageWidth
				    || image.Height > _settings.Options.MaxUploadImageHeight)
				{
					var resizedImage = Resize(image);
					resizedImage.Save(filePath, ImageFormat.Png);
				}
				else
				{
					image.Save(filePath);
				}
			}

			return new JsonResult(fileName);
		}

		[HttpPost]
		public ActionResult AddMedia(IFormFile theFile)
		{
			if (theFile == null || theFile.Length <= 0)
				return HttpBadRequest();

			var uploads = GetUploadsFolderPath();

			var fileName = ContentDispositionHeaderValue.Parse(theFile.ContentDisposition).FileName.Trim('"');
			var filePath = Path.Combine(uploads, fileName);
			theFile.SaveAs(filePath);

			return new JsonResult(fileName);
		}

		private Image Resize(Image image)
	    {
		    int originalWidth = image.Width;
		    int originalHeight = image.Height;
		    float percentWidth = (float)_settings.Options.MaxUploadImageWidth / (float) originalWidth;
		    float percentHeight = (float)_settings.Options.MaxUploadImageHeight /(float) originalHeight;
		    float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
		    var newWidth = (int) (originalWidth*percent);
		    var newHeight = (int) (originalHeight*percent);
		    var newImage = new Bitmap(newWidth, newHeight);

		    using (var graphicsHandle = Graphics.FromImage(newImage))
		    {
			    graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
			    graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
		    }

		    return newImage;
	    }

	    private string GetUploadsFolderPath()
	    {
		    var uploads = Path.Combine(_environment.WebRootPath, "uploads");

		    if (!Directory.Exists(uploads))
			    Directory.CreateDirectory(uploads);
		    return uploads;
	    }

		private string GetAvatarsFolderPath()
		{
			var uploads = Path.Combine(_environment.WebRootPath, "avatars");

			if (!Directory.Exists(uploads))
				Directory.CreateDirectory(uploads);

			return uploads;
		}
	}
}
