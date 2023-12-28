namespace ServicesApp.APIs
{
    public static  class ApiResponse
    {
        public static readonly object NotFoundUser = new
        {
            statusMsg = "fail",
            message = "Not Found."
        };
        public static readonly object NotValid= new
        {
            statusMsg = "fail",
            message = "Not Valid."
        };
        public static readonly object SomthingWronge = new
        {
            statusMsg = "fail",
            message = "Something went Wrong."
        };
        public static readonly object SuccessDeleted = new
        {
            statusMsg = "success",
            message = "Successfully Deleted."
        };
        public static readonly object SuccessUpdated = new
        {
            statusMsg = "success",
            message = "Successfully Updated."
        };
        public static readonly object UserAlreadyExist = new
        {
            statusMsg = "fail",
            message = "User Already Exists."
        };
        public static readonly object RoleDoesNotExist = new
        {
            statusMsg = "fail",
            message = "Role Doesn't Exist."
        };
        public static readonly object UnAutharized = new
        {
            statusMsg = "fail",
            message = "Invalid Email or Password."
        };
        public static readonly object CanNotSentMail = new
        {
            statusMsg = "fail",
            message = "Can not send mail"
        };
        public static readonly object PassChanged = new
        {
            statusMsg = "success",
            message = "Password Changed Successfully."
        };
        public static readonly object CanNotChangePass = new
        {
            statusMsg = "fail",
            message = "Can not Change Password."
        };
        public static readonly object CategoryNotFound = new
        {
            statusMsg = "fail",
            message = "Category Not Found."
        };
        public static readonly object CategoryAlreadyExist= new
        {
            statusMsg = "fail",
            message = "Category Already Exists."
        };
        public static readonly object OfferNotFound = new
        {
            statusMsg = "fail",
            message = "Offer Not Found."
        };
        public static readonly object RequestNotFound = new
        {
            statusMsg = "fail",
            message = "Service Request Not Found."
        };
        public static readonly object TimeSlotNotFound = new
        {
            statusMsg = "fail",
            message = "Time Slot is not Found."
        };
        public static readonly object FailedUpdated = new
        {
            statusMsg = "fail",
            message = "Failed to Update."
        };
        public static readonly object TimeSlotConflict = new
        {
            statusMsg = "fail",
            message = "Conflict in time slots."
        };
        public static readonly object OfferAccepted = new
        {
            statusMsg = "success",
            message = "Offer Accepted."
        };
        public static readonly object ServiceCompletedSuccess = new
        {
            statusMsg = "success",
            message = "Service Completed Successfully."
        };
        public static readonly object CreatedSuccess = new
        {
            statusMsg = "success",
            message = "Created Successfully."
        };





    }
}
