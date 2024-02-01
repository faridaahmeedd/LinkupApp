﻿namespace ServicesApp.APIs
{
    public static  class ApiResponse
    {
        public static readonly object NotValid = new
        {
            statusMsg = "fail",
            message = "Not Valid."
        };
        public static readonly object SomethingWrong = new
        {
            statusMsg = "fail",
            message = "Something went Wrong."
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
        public static readonly object Unuthorized = new
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
		public static readonly object CategoryAlreadyExist= new
        {
            statusMsg = "fail",
            message = "Category Already Exists."
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
        public static readonly object OfferDeclined = new
        {
            statusMsg = "success",
            message = "Offer Declined."
        };
        public static readonly object ServiceCompletedSuccess = new
        {
            statusMsg = "success",
            message = "Service Completed Successfully."
        };
        public static readonly object AlreadyOffered = new
        {
            statusMsg = "fail",
            message = "You are Already Offered this Service."
        };
        public static readonly object InvalidPass = new
        {
            statusMsg = "fail",
            message = "Invalid Password."
        };
        public static readonly object PayFine = new
        {
            statusMsg = "fail",
            message = "Please Pay For the Previous Service First."
        };
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
        public static readonly object FeesExceededMax = new
        {
            statusMsg = "fail",
            message = "Offer Fees exceeded Request max fees"
        };
    }
}
