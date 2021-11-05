namespace MvcNotePortalApp.Entities.Messages
{
    public enum ErrorMessage
    {
        UsernameAlreadyExists = 101,
        EmailAlreadyExists = 102,
        UserIsNotActive = 121,
        UserNotFound = 131,
        UsernameOrPasswordWrong = 141,
        CheckYourMail = 151,
        UserAlreadyActive = 152,
        ActivateCodeInvalid = 153,
        ProfileCouldNotUpdated = 154,
        UserCouldNotRemove = 155,
        UserCouldNotInserted = 156
    }
}
