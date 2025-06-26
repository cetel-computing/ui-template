namespace FlazorTemplate.Services.Modals
{
    /// <summary>
    /// Facade to provide shared modal actions
    /// </summary>
    public interface IModalActions
    {
        Task ShowFeedbackDialog(string email);
    }
}
