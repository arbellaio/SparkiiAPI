using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectApi.AppProperties
{
    public class AppStrings
    {
        public static string UserRoleUndefined = "User role undefined.";
        public static string UserAlreadyExistsWithEmailAddress = "Account already exists with this email address.";
        public static string UserAlreadyExistsWithPhoneNumber = "Account already exists with this phone number.";
        public static string IncompleteOrInvalidInformation = "Incomplete Or Invalid Information.";
        public static string UserDoesntExist = "User doesn't exist.";
        public static string UserAttemptingLogin = "User atttempting login.";
        public static string UserLoginSuccessful = "User logged-in successfully.";
        public static string UserRequestingForOtp = "User requesting for OTP from phone number.";
        public static string UserOtpRequestFailed = "User OTP Request Failed.";
        public static string UserEnteredIncorrectPhoneNumber = "User Entered Incorrect PhoneNumber.";
        public static string UserOtpGenerationSuccessful = "User Otp generated Successfully.";
        public static string UserEnteringPhoneNumberForOtpResend = "User Entering PhoneNumber for Resending OTP.";
        public static string OtpSmsSentToUser = "OTP Sms has been sent to user.";
        public static string UserRegisteringAccount = "User is trying to register his/her account.";
        public static string UserRegisteredSuccessfully = "User registered successfully.";
        public static string AttemptingToAssignRoleUser = "Attempting to assigning role to User.";
        public static string UserRoleAssigned = "User role assigned.";
        public static string GetAllSupportUsers = "Getting all support-users against user";
        public static string SendingSmsAboutAvailabilityToAllSupport = "Sending sms about availability of user to all supports";
        public static string SendingSmsAboutAvailabilityToAllGuest = "Sending sms about availability of user to all guests";
        public static string GetAllGuestUsers = "Getting all guest-users against user";
        public static string CreateChatAccount = "Getting all guest-users against user";
        public static string UserAttemptingToGetAllSupport = "User accessing api to get all support members";
        public static string UserAttemptingToGetAllPatient = "User accessing api to get all support members";
        public static string NoSupportMembersForthisUser = "No support members for this user";
        public static string UserAttemptToGetAllSupportMembersSuccess = "User successfully obtained list of all support members";
        public static string UserAttemptToGetAllPatientMembersSuccess = "User successfully obtained list of all guest members";
        public static string UserAccessingInviteApi = "User successfully obtained list of all guest members";
        public static string UserInfoUpdated = "User info updated successfully.";
        public static string AttemptingUserUpdate = "Attempting to update user info";
        public static string UserAttemptingToRegister = "User attempting to Register.";
        public static string AttemptingToGetUserInfoByPhoneNumber = "Attempting to get user info by phone number.";
        public static string UserInfoAccessUserInvited = "User info accessed user was invited by someone, User needs to register to access app.";



        public static string EmailOrPasswordInvalid = "Email or Password invalid";
        public static string InvalidEmail = "Invalid Email Format";
        public static string OtpCodeAttemptsExceeded = "OTP code attempts exceeded";
        public static string OtpTimeExpired = "OTP time expired";
        public static string WrongOtp = "Wrong OTP";
        public static string PhoneNumberOrCountryCodeEmpty = "Phone Number empty";
        public static string InvalidPhoneNumberOrCountryCode = "Phone Number invalid";
        public static string ConnectApi = "ConnectApi Code : ";
        public static string UserIdisInvalid = "User Id is invalid.";
        public static string UserDoesntHaveAnySupport = "User doesn't have any support members.";
        public static string UserDoesntHaveAnyPatient = "User doesn't have any patients.";
        public static string UserCannotBeAdded = "User cannot be added contact admin.";
        public static string Patient = "Patient";
        public static string Support = "Support";
        public const string Admin = "Admin";
        public static string Role = "";
        public static string UserAlreadyInvited = "User has already been invited.";


        public static string UserCantBeInvited = "User cannot be invited, Invalid or Incorrect info.";
        public static string UserInvitedSuccess = "User invited successfully.";
        public static string SendingSmsToInvitedUser = "Sending sms to invited user.";
        public static string SendingSmsToInviteeUser = "Sending sms to invitee user.";

        public static string InvitationMessage1 = "Invitation : ";
        public static string InvitationMessage2 = " Wants you to join RiseUp as ";
        public static string InvitationMessage3 = " for him / her.";

        public static string InviteeUserMessage1 = "Invitation has been sent to ";
        public static string InviteeUserMessage2 = " you will be connected when he / she accepts the invitation.";

        public static string InvitationAccepted = " has accepted your invitation.";

        public static string CountryCode = "+1";
        public static string InviteeUserMessage1IsRegister = "User ";
        public static string InviteeUserMessage2IsRegister = " has been connected as your ";


        public static string QuestionPropertiesInvalid = "Question Title Or Category, ResponseType, Type Undefined";
        public static string InvalidAnswerInput = "Answer Input Required";
        public static string SomethingWentWrong = "Something went wrong. Please try again.";
        public static string QuestionAndAnswer = "Question Answers Added Successfully.";



    }
}
