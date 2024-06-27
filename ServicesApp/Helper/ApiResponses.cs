namespace ServicesApp.Helper
{
    public static class ApiResponses
    {
        public static readonly object NotValid = new
        {
            statusMsg = "fail",
            message = "Not Valid."
        };
        public static readonly object SomethingWrong = new
        {
            statusMsg = "fail",
            message = "Something Went Wrong."
        };
        public static readonly object SuccessCreated = new
        {
            statusMsg = "success",
            message = "Created Successfully."
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
        public static readonly object FailedToUpdate = new
        {
            statusMsg = "fail",
            message = "Failed to Update."
        };
		public static readonly object FailedToDelete = new
		{
			statusMsg = "fail",
			message = "Failed to Delete."
		};
		public static readonly object Unauthorized = new
        {
            statusMsg = "fail",
            message = "Invalid Email or Password."
        };
        public static readonly object CanNotSentMail = new
        {
            statusMsg = "fail",
            message = "Can not send mail."
        };
        public static readonly object PassChanged = new
        {
            statusMsg = "success",
            message = "Password Changed Successfully."
        };
        public static readonly object UserDeactivated = new
        {
            statusMsg = "success",
            message = "User Deactivated Successfully."
        };
        public static readonly object CanNotChangePass = new
        {
            statusMsg = "fail",
            message = "Can not Change Password."
        };
        public static readonly object UserAlreadyExist = new
        {
            statusMsg = "fail",
            message = "User Already Exists."
        };
        public static readonly object AdminAlreadyExist = new
        {
            statusMsg = "fail",
            message = "Admin Already Exists."
        };
        public static readonly object CategoryAlreadyExist = new
        {
            statusMsg = "fail",
            message = "Category Already Exists."
        };
        public static readonly object SubcategoryAlreadyExist = new
        {
            statusMsg = "fail",
            message = "SubCategory Already Exists."
        };
        public static readonly object RoleDoesNotExist = new
        {
            statusMsg = "fail",
            message = "Role Doesn't Exist."
        };
        public static readonly object UserNotFound = new
        {
            statusMsg = "fail",
            message = "User Not Found."
        };
        public static readonly object CategoryNotFound = new
        {
            statusMsg = "fail",
            message = "Category Not Found."
        };
        public static readonly object SubcategoryNotFound = new
        {
            statusMsg = "fail",
            message = "SubCategory Not Found."
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
            message = "Time Slot Not Found."
        };
        public static readonly object TimeSlotConflict = new
        {
            statusMsg = "fail",
            message = "Conflict in time slots."
        };
        public static readonly object OfferAccepted = new
        {
            statusMsg = "success",
            message = "Offer Accepted Successfully."
        };
        public static readonly object OfferDeclined = new
        {
            statusMsg = "success",
            message = "Offer Declined Successfully."
        };
        public static readonly object ServiceCompletedSuccess = new
        {
            statusMsg = "success",
            message = "Service Completed Successfully."
        };
        public static readonly object AlreadyOffered = new
        {
            statusMsg = "fail",
            message = "You have already offered on this request."
        };
        public static readonly object InvalidPass = new
        {
            statusMsg = "fail",
            message = "Invalid Password."
        };
        //public static readonly object PayBalance = new
        //{
        //    statusMsg = "fail",
        //    message = "Please Pay For the Previous Service First."
        //};
        public static readonly object ProviderCanOffer = new
        {
            statusMsg = "success",
            message = "Provider Can Offer."
        };
        public static readonly object TimeSlotsExceededMax = new
        {
            statusMsg = "fail",
            message = "You can only add up to three time slots."
        };
		public static readonly object ImagesExceededMax = new
		{
			statusMsg = "fail",
			message = "You can only add up to five images."
		};
		public static readonly object FeesOutsideRange = new
        {
            statusMsg = "fail",
            message = "Offered fees is outside the category's specified range."
        };
        public static readonly object NotAuthorized = new
        {
            statusMsg = "fail",
            message = "Not authorized to do this action."
        };
        public static readonly object ReviewNotFound = new
        {
            statusMsg = "fail",
            message = "Review not Found."
        };
		public static readonly object InvalidRating = new
		{
			statusMsg = "fail",
			message = "The rating value is out of the expected range (0-5)."
		};
		public static readonly object ServiceAlreadyReviewed = new
		{
			statusMsg = "fail",
			message = "This service has already been reviewed."
		};
		public static readonly object ReportNotFound = new
        {
            statusMsg = "fail",
            message = "Report not Found."
        };
		public static readonly object PaymentError = new
		{
			statusMsg = "fail",
			message = "Error occured while paying for the service."
		};
		public static readonly object PaidAlready = new
        {
            statusMsg = "fail",
            message = "This service has already been paid."
        };
        public static readonly object CannotCapture = new
        {
            statusMsg = "fail",
            message = "Can not capture this transaction."
        };
        public static readonly object CaptureSuccess = new
        {
            statusMsg = "success",
            message = "This service has been capture successfully."
        };
        public static readonly object UncompletedService = new
        {
            statusMsg = "fail",
            message = "This service is not completed."
        };
		public static readonly object NotExamination = new
		{
			statusMsg = "fail",
			message = "Not an examination visit."
		};

		//public static readonly object RefundedAlready = new
		//{
		//	statusMsg = "fail",
		//	message = "This service has already been refunded."
		//};
		//public static readonly object RefundSuccess = new
		//{
		//	statusMsg = "success",
		//	message = "This service has been refunded successfully."
		//};
	}
}