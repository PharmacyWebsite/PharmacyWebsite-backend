namespace PharmacyOrderingSystem.Helpers
{
    public class FileUploadHelper
    {
        public async Task<string> UploadAsync(IFormFile file)
        {
            try
            {
                var path = Path.Combine("Uploads", file.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                Console.WriteLine($"[FILE] Uploaded: {path}");

                return path;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FILE ERROR] {ex.Message}");
                throw;
            }
        }
    }
}