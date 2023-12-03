using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Services
{
    public class Base64ImageConverterService
    {
        public static async Task<string> PickedImageToBase64Async()
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images
            });

            if (result is null)
            {
                return null;
            }
            // extract below to extension method or helper class for base64
            var imageStream = await result.OpenReadAsync();
            var memoryStreamPicked = new MemoryStream();
            await imageStream.CopyToAsync(memoryStreamPicked);
            //AlterPlant.ImageLocation = Convert.ToBase64String(memoryStream.ToArray());
            return Convert.ToBase64String(memoryStreamPicked.ToArray());
        }

        public static ImageSource Base64ToImage(string base64String)
        {
            var bytesOfImage = Convert.FromBase64String(base64String);
            MemoryStream memoryStreamPlaced = new MemoryStream(bytesOfImage);
            return ImageSource.FromStream(() => memoryStreamPlaced);
        }
    }
}