namespace PharmacyOrderingSystem.Helpers
{
    public class EmailHelper
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                Console.WriteLine($"[EMAIL] Sending email to {to}");

                // Simulated email logic
                await Task.Delay(500);

                Console.WriteLine($"[EMAIL] Email sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] {ex.Message}");
                throw;
            }
        }
    }
}