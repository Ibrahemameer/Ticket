namespace RhinoTicketingSystem.Services
{
    public class CultureService
    {
        public string CurrentCulture { get; set; } = "en"; // Default to English

        public void SetCulture(string culture)
        {
            CurrentCulture = culture;
        }

    }
}
