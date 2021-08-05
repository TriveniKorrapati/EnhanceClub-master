using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.CognitoIdentityProvider.Model.Internal.MarshallTransformations;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using EnhanceClub.Domain.AwsEntities;
using EnhanceClub.Domain.AwsHelper;
using EnhanceClub.Domain.Entities;
using EnhanceClub.WebUI.Infrastructure.Utility;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace EnhanceClub.WebUI.AwsHelper
{
    public class CognitoHelper
    {
        static readonly Amazon.RegionEndpoint Region = Amazon.RegionEndpoint.CACentral1;

        static AWSCredentials _awsCredentials = new BasicAWSCredentials(AwsConfiguration.CognitoAccessKeyId, AwsConfiguration.CognitoSecretAccessKey);

        private static string _accessToken;

        private static string _sessionId;

        private static readonly string PoolId = AwsConfiguration.PoolId;

        static readonly string ClientAppId = AwsConfiguration.ClientId;

        readonly AmazonCognitoIdentityProviderClient _provider = new AmazonCognitoIdentityProviderClient(_awsCredentials, Region);

        public static CognitoSignupResponse AwsSignUpUser(string email, string password, string phone)
        {
            AmazonCognitoIdentityProviderClient provider =
                new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), Region);

            string[] emailUsername = email.Split('@');

            // uncomment this method, if need to replace '@' with any other char
            // var userName = Utility.BuildCognitoUserName(email, 1);

            // generate secret hash from app client secret (WE are not using app client secret )
            // var secretHash = CognitoHashCalculator.GetSecretHash(emailUsername[0], ClientAppId, AppClientSecret);

            SignUpRequest signUpRequest = new SignUpRequest();

            signUpRequest.ClientId = ClientAppId;
            signUpRequest.Password = password;
            signUpRequest.Username = email;

            // signUpRequest.SecretHash = secretHash;


            List<AttributeType> attributes = new List<AttributeType>()
            {
                new AttributeType(){Name = "email", Value = email},
                new AttributeType(){Name = "phone_number", Value = phone}
            };
            signUpRequest.UserAttributes = attributes;

            CognitoSignupResponse signupResponse = null;

            try
            {
                SignUpResponse result = provider.SignUp(signUpRequest);

                signupResponse = new CognitoSignupResponse
                {
                    UserCreated = true,
                    HttpStatusCode = result.HttpStatusCode,
                    CodeDeliveryMethod = result.CodeDeliveryDetails.Destination,
                    UserConfirmed = result.UserConfirmed,
                    UserSub = result.UserSub

                };
                return signupResponse;

            }
            catch (Exception e)
            {
                var exceptionType = e.GetType().Name;
                signupResponse = new CognitoSignupResponse
                {
                    UserCreated = false,
                    Message = e.Message,
                    Exception = e.InnerException == null ? string.Empty : e.InnerException.ToString(),
                    ErrorCode = exceptionType
                };
                return signupResponse;
            }
        }

        public static CognitoConfirmSignupResponse VerifyAccessCode(string email, string code)
        {
            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(AwsConfiguration.CognitoAccessKeyId, AwsConfiguration.CognitoSecretAccessKey, Region);

            ConfirmSignUpRequest confirmSignUpRequest = new ConfirmSignUpRequest();

            //string[] emailUsername = email.Split('@');
            confirmSignUpRequest.Username = email;
            confirmSignUpRequest.ConfirmationCode = code;
            confirmSignUpRequest.ClientId = ClientAppId;
            CognitoConfirmSignupResponse cognitoConfirmSignupResponse = null;
            try
            {
                ConfirmSignUpResponse confirmSignUpResult = provider.ConfirmSignUp(confirmSignUpRequest);
                var test = confirmSignUpResult;
                cognitoConfirmSignupResponse = new CognitoConfirmSignupResponse
                {
                    AccessCode = code,
                    AccessCodeVerified = true,
                    HttpStatusCode = confirmSignUpResult.HttpStatusCode
                };
                return cognitoConfirmSignupResponse;

            }
            catch (Exception e)
            {
                cognitoConfirmSignupResponse = new CognitoConfirmSignupResponse
                {
                    AccessCode = code,
                    AccessCodeVerified = false,
                    Message = e.Message,
                    Exception = e.InnerException == null ? string.Empty : e.InnerException.ToString(),
                };
                return cognitoConfirmSignupResponse;
            }
        }

        public static UserAttributeResponse UpdateUserAttributes(string email, string accessToken, CustomerSignUp customerInfo)
        {
            // AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), Region);

            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(AwsConfiguration.CognitoAccessKeyId, AwsConfiguration.CognitoSecretAccessKey, Region);

            UpdateUserAttributesRequest updateUserAttributesRequest = new UpdateUserAttributesRequest();

            //string[] emailUsername = email.Split('@');

            List<AttributeType> attributes = new List<AttributeType>()
            {

               new AttributeType(){Name = "custom:CustomerCity", Value = customerInfo.CustomerCity},
               new AttributeType(){Name = "custom:CustomerZipCode", Value = customerInfo.CustomerZipCode},
                new AttributeType(){Name = "custom:CustomerAddress", Value = customerInfo.CustomerAddress},
                //new AttributeType(){Name = "custom:CustomerPhone", Value = customerInfo.CustomerPhone},
                new AttributeType(){Name = "custom:CustomerFirstName", Value = customerInfo.CustomerFirstName},
                new AttributeType(){Name = "custom:CustomerLastName", Value = customerInfo.CustomerLastName},
                 new AttributeType(){Name = "custom:CustomerProvinceFk", Value = customerInfo.CustomerProvinceFk.ToString()},
            };
            updateUserAttributesRequest.UserAttributes = attributes;
            updateUserAttributesRequest.AccessToken = accessToken;
            try
            {
                UpdateUserAttributesResponse updateUserAttributesResponse = provider.UpdateUserAttributes(updateUserAttributesRequest);

                var userAttributeResponse = new UserAttributeResponse
                {
                    HttpStatusCode = updateUserAttributesResponse.HttpStatusCode,
                    AccessToken = accessToken
                };

                return userAttributeResponse;

                //return updateUserAttributesResponse;

            }
            catch (AggregateException e)
            {
                var userAttributeResponse = new UserAttributeResponse
                {
                    AccessToken = accessToken,
                    Message = e.Message,
                    Exception = e.InnerException == null ? string.Empty : e.InnerException.ToString()
                };
                return userAttributeResponse;
                //throw e;
            }
            catch (Exception e)
            {
                var userAttributeResponse = new UserAttributeResponse
                {
                    AccessToken = accessToken,
                    Message = e.Message,
                    Exception = e.InnerException == null ? string.Empty : e.InnerException.ToString()
                };
                return userAttributeResponse;

            }
        }

        public static async Task<SrpAuthResponse> GetCredentialsAsync(string email, string password)
        {
            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(AwsConfiguration.CognitoAccessKeyId, AwsConfiguration.CognitoSecretAccessKey, Region);

            CognitoUserPool userPool = new CognitoUserPool(PoolId, ClientAppId, provider);

            // string[] emailUsername = email.Split('@');

            CognitoUser user = new CognitoUser(email, ClientAppId, userPool, provider);

            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
            {
                Password = password

            };
            try
            {
                AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);

                _sessionId = authResponse.SessionID;

                var srpAuthResponse = new SrpAuthResponse
                {
                    SessionID = _sessionId,
                    CustomerEmail = email
                    
                };

                return srpAuthResponse;
            }
            catch (UserNotConfirmedException ex)
            {
                var srpAuthResponse = new SrpAuthResponse
                {
                    SessionID = _sessionId,
                    CustomerEmail = email,
                    Message = ex.Message,
                    Exception = ex.InnerException == null ? string.Empty : ex.InnerException.ToString(),
                    UserNotConfirmed = true
                };

                return srpAuthResponse;
            }

            catch (Exception e)
            {
                var srpAuthResponse = new SrpAuthResponse
                {
                    SessionID = _sessionId,
                    CustomerEmail = email,
                    Message = e.Message,
                    Exception = e.InnerException == null ? string.Empty : e.InnerException.ToString(),
                };

                return srpAuthResponse;
            }
        }

        public virtual async Task<AuthFlowMfaResponse> RespondToTwoFactorChallengeAsync(string email, string code, string authWorkflowSessionId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(AwsConfiguration.CognitoAccessKeyId, AwsConfiguration.CognitoSecretAccessKey, Region);
            //string[] emailUsername = email.Split('@');

            CognitoUserPool userPool = new CognitoUserPool(PoolId, ClientAppId, provider);

            CognitoUser user = new CognitoUser(email, ClientAppId, userPool, provider);

            try
            {
                AuthFlowResponse context =
                    await user.RespondToSmsMfaAuthAsync(new RespondToSmsMfaRequest()
                    {
                        SessionID = authWorkflowSessionId,
                        MfaCode = code,

                    }).ConfigureAwait(false);

                var userId = Task.FromResult(context).Id;

                var authFlowDetails = new AuthFlowMfaResponse
                {
                    AccessToken = context.AuthenticationResult != null ? context.AuthenticationResult.AccessToken : string.Empty,
                    UserId = userId,
                    IdToken = context.AuthenticationResult != null ? context.AuthenticationResult.IdToken : string.Empty,
                    ExpiresIn = context.AuthenticationResult != null ? context.AuthenticationResult.ExpiresIn : 0,
                };

                var accessToken = context.AuthenticationResult.AccessToken;
                return authFlowDetails;
                //return context;
            }
            catch (AmazonCognitoIdentityProviderException e)
            {
                var authFlowDetails = new AuthFlowMfaResponse
                {
                    Message = e.Message,
                    Exception = e.InnerException == null ? string.Empty : e.InnerException.ToString(),

                };
                return authFlowDetails;
                //throw new CognitoServiceException("Failed to respond to Cognito two factor challenge.", e);
            }
        }

        public static List<AttributeType> GetUserAttributes(string accessToken)
        {
            //AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), Region);

            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(AwsConfiguration.CognitoAccessKeyId, AwsConfiguration.CognitoSecretAccessKey, Region);

            GetUserRequest getUser = new GetUserRequest();
            getUser.AccessToken = accessToken;

            GetUserResponse userData = new GetUserResponse();

            userData = provider.GetUser(getUser);

            List<AttributeType> attributes = new List<AttributeType>();
            try
            {
                attributes = provider.GetUser(getUser).UserAttributes;
                return attributes;

            }
            catch (AggregateException e)
            {
                throw e;
            }
        }

        public static List<AttributeType> GetUser(string email)
        {
            //AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(_awsCredentials, Region);

            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(AwsConfiguration.CognitoAccessKeyId, AwsConfiguration.CognitoSecretAccessKey, Region);

            CognitoUserPool userPool = new CognitoUserPool(PoolId, ClientAppId, provider);
            AdminGetUserRequest adminGetUserRequest = new AdminGetUserRequest();

            adminGetUserRequest.Username = email;
            adminGetUserRequest.UserPoolId = userPool.PoolID;

            try
            {
                AdminGetUserResponse adminGetUserResponse = new AdminGetUserResponse();
                adminGetUserResponse = provider.AdminGetUser(adminGetUserRequest);
                return adminGetUserResponse.UserAttributes;
            }
            catch (AggregateException e)
            {
                throw e;
            }
        }

        public static CognitoSignupResponse ResetPassword(string email)
        {
            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(AwsConfiguration.CognitoAccessKeyId, AwsConfiguration.CognitoSecretAccessKey, Region);
           
            // string[] emailUserName = email.Split('@');
            ForgotPasswordRequest forgotPasswordRequest = new ForgotPasswordRequest()
            {
                Username = email
            };
            forgotPasswordRequest.ClientId = ClientAppId;
            CognitoSignupResponse forgotPwdResponse = null;
            try
            {
                ForgotPasswordResponse forgotPasswordResponse = provider.ForgotPassword(forgotPasswordRequest);
                forgotPwdResponse = new CognitoSignupResponse
                {
                    HttpStatusCode = forgotPasswordResponse.HttpStatusCode
                };
                return forgotPwdResponse;
                // return forgotPasswordResponse;
            }
            catch (AggregateException e)
            {
                throw e;
            }
        }

        public static CognitoSignupResponse VerifyResetCode(string email, string code, string newPassword)
        {
            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(AwsConfiguration.CognitoAccessKeyId, AwsConfiguration.CognitoSecretAccessKey, Region);

            ConfirmForgotPasswordRequest confirmForgotPasswordRequest = new ConfirmForgotPasswordRequest();

            // string[] emailUsername = email.Split('@');


            confirmForgotPasswordRequest.Username = email;
            confirmForgotPasswordRequest.ConfirmationCode = code;
            confirmForgotPasswordRequest.ClientId = ClientAppId;
            confirmForgotPasswordRequest.Password = newPassword;
            CognitoSignupResponse forgotPwdResponse = null;
            try
            {
                ConfirmForgotPasswordResponse confirmedPasswordChange = provider.ConfirmForgotPassword(confirmForgotPasswordRequest);
                forgotPwdResponse = new CognitoSignupResponse
                {
                    HttpStatusCode = confirmedPasswordChange.HttpStatusCode
                };
                return forgotPwdResponse;
                // return confirmedPasswordChange;
            }
            catch (AggregateException e)
            {
                forgotPwdResponse = new CognitoSignupResponse
                {
                    Message = e.Message,
                    Exception = e.InnerException == null ? string.Empty : e.InnerException.ToString()

                };
                return forgotPwdResponse;
            }
            catch (Exception e)
            {
                forgotPwdResponse = new CognitoSignupResponse
                {
                    Message = e.Message,
                    Exception = e.InnerException == null ? string.Empty : e.InnerException.ToString()

                };
                return forgotPwdResponse;
            }
        }

        public static CognitoSignupResponse ResendConfirmationEmail(string email)
        {
            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(AwsConfiguration.CognitoAccessKeyId, AwsConfiguration.CognitoSecretAccessKey, Region);

            ResendConfirmationCodeRequest resendCodeRequest = new ResendConfirmationCodeRequest();
            resendCodeRequest.Username = email;
            resendCodeRequest.ClientId = ClientAppId;
            CognitoSignupResponse signupResponse = null;

            try
            {
                ResendConfirmationCodeResponse confirmedPasswordChange = provider.ResendConfirmationCode(resendCodeRequest);
                signupResponse = new CognitoSignupResponse
                {
                    HttpStatusCode = confirmedPasswordChange.HttpStatusCode
                };
                return signupResponse;
            }
            catch (AggregateException e)
            {
                signupResponse = new CognitoSignupResponse
                {
                    Message = e.Message,
                    Exception = e.InnerException == null ? string.Empty : e.InnerException.ToString()

                };
                return signupResponse;
            }
            catch (Exception e)
            {
                signupResponse = new CognitoSignupResponse
                {
                    Message = e.Message,
                    Exception = e.InnerException == null ? string.Empty : e.InnerException.ToString()

                };
                return signupResponse;
            }


        }
        public static async Task<string> AuthenticateUser(string email, string password)
        {
            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(AwsConfiguration.CognitoAccessKeyId, AwsConfiguration.CognitoSecretAccessKey, Region);

            CognitoUserPool userPool = new CognitoUserPool(PoolId, ClientAppId, provider);

            CognitoUser user = new CognitoUser(email, ClientAppId, userPool, provider);

            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
            {
                Password = password
            };


            AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);

            _accessToken = authResponse.AuthenticationResult.AccessToken;

            return _accessToken;
        }

        public async void GetCredentialsFromRefreshAsync(string refreshToken, string email)
        {
            AmazonCognitoIdentityProviderClient provider =
                new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), FallbackRegionFactory.GetRegionEndpoint());

            CognitoUserPool userPool = new CognitoUserPool(PoolId, ClientAppId, provider);

            string[] emailUserName = email.Split('@');

            CognitoUser user = new CognitoUser(emailUserName[0], ClientAppId, userPool, provider);
            user.SessionTokens = new CognitoUserSession(null, null, refreshToken, DateTime.Now, DateTime.Now.AddHours(1));
            InitiateRefreshTokenAuthRequest refreshRequest = new InitiateRefreshTokenAuthRequest()
            {
                AuthFlowType = AuthFlowType.REFRESH_TOKEN_AUTH
            };
            AuthFlowResponse authResponse = await user.StartWithRefreshTokenAuthAsync(refreshRequest).ConfigureAwait(false);
        }

        public static void LogoutUser(string accessToken, string email)
        {
            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), Region);

            AdminUserGlobalSignOutRequest adminUserGlobalSignOutRequest = new AdminUserGlobalSignOutRequest();

            string[] emailUserName = email.Split('@');
            adminUserGlobalSignOutRequest.Username = emailUserName[0];
            adminUserGlobalSignOutRequest.UserPoolId = PoolId;

            provider.AdminUserGlobalSignOutAsync(adminUserGlobalSignOutRequest);


        }

        public static List<UserType> GetAllUsers()
        {
            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(_awsCredentials, Region);

            try
            {
                List<UserType> users = new List<UserType>();
                bool continueListing = true;
                ListUsersResponse response = provider.ListUsers(
                    new ListUsersRequest
                    {
                        UserPoolId = PoolId,
                        Limit = 60,
                        AttributesToGet = new List<string>
                        {
                            "email"
                        }
                    });
                users.AddRange(response.Users);
                string paginationToken = response.PaginationToken;
                while (continueListing)
                {
                    response = provider.ListUsers(
                        new ListUsersRequest
                        {
                            UserPoolId = PoolId,
                            Limit = 60,
                            PaginationToken = paginationToken,
                            AttributesToGet = new List<string>
                            {
                                "email"
                            }
                        });
                    if (response.Users.Count < 60)
                    {
                        continueListing = false;
                    }
                    else
                    {
                        paginationToken = response.PaginationToken;
                        users.AddRange(response.Users);
                    }
                }

                return users;
            }
            catch (Exception)
            {
                //_session.NotifyUser(Notification.GeneralError());
                //_logger.LogError(e.Message);
            }
            return null;

        }

        public static AdminUpdateAuthEventFeedbackResponse AdminUpdateAuthEventFeedback(string email)
        {
            AmazonCognitoIdentityProviderClient provider =
                          new AmazonCognitoIdentityProviderClient(_awsCredentials, Region);
            CognitoUserPool userPool = new CognitoUserPool(PoolId, ClientAppId, provider);

            AdminUpdateAuthEventFeedbackRequest adminUpdateAuthEventFeedbackRequest = new AdminUpdateAuthEventFeedbackRequest();
            AdminUpdateAuthEventFeedbackResponse adminUpdateAuthEventFeedbackResponse = new AdminUpdateAuthEventFeedbackResponse();

            string[] emailUserName = email.Split('@');

            adminUpdateAuthEventFeedbackRequest.Username = emailUserName[0];
            adminUpdateAuthEventFeedbackRequest.UserPoolId = PoolId;
            provider.AdminUpdateAuthEventFeedback(adminUpdateAuthEventFeedbackRequest);
            return adminUpdateAuthEventFeedbackResponse;

        }

        public static async Task<bool> CheckPasswordAsync(string userName, string password)
        {
            try
            {
                AmazonCognitoIdentityProviderClient provider =
                new AmazonCognitoIdentityProviderClient(_awsCredentials, Region);

                var authReq = new AdminInitiateAuthRequest
                {
                    UserPoolId = PoolId,
                    ClientId = ClientAppId,
                    AuthFlow = AuthFlowType.ADMIN_USER_PASSWORD_AUTH,
                };

                authReq.AuthParameters.Add("USERNAME", userName);
                authReq.AuthParameters.Add("PASSWORD", password);

                // var test = GetCredentialsAsync(userName, password);

                AdminInitiateAuthResponse authResp = await provider.AdminInitiateAuthAsync(authReq);

                var responseObject = new SrpAuthResponse
                {
                    AccessToken = authResp.AuthenticationResult.AccessToken,
                    SessionID = authResp.Session

                };

                return true;
            }
            catch (Exception e)
            {
                var responseObject = new SrpAuthResponse
                {
                    Message = e.Message,
                    Exception = e.InnerException != null ? e.InnerException.ToString() : string.Empty
                };
                return false;
            }
        }

        // test method to delete user
        public static AdminDeleteUserResponse DeleteUser(string email)
        {
            AmazonCognitoIdentityProviderClient provider =
                                            new AmazonCognitoIdentityProviderClient(_awsCredentials, Region);
            AdminDeleteUserRequest delReq = new AdminDeleteUserRequest
            {
                Username = email,
                UserPoolId = PoolId
            };
            try
            {
                var response = provider.AdminDeleteUser(delReq);
                return response;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static UserAttributeResponse UpdateUserEmail(string newEmail, string accessToken)
        {
            AmazonCognitoIdentityProviderClient provider =
                                          new AmazonCognitoIdentityProviderClient(_awsCredentials, Region);
            UpdateUserAttributesRequest updateUserAttributesRequest = new UpdateUserAttributesRequest();

            //string[] emailUsername = email.Split('@');

            List<AttributeType> attributes = new List<AttributeType>()
            {

               new AttributeType(){Name = "email", Value = newEmail},
              
            };
            updateUserAttributesRequest.UserAttributes = attributes;
            updateUserAttributesRequest.AccessToken = accessToken;
            try
            {
                UpdateUserAttributesResponse updateUserAttributesResponse = provider.UpdateUserAttributes(updateUserAttributesRequest);

                var userAttributeResponse = new UserAttributeResponse
                {
                    HttpStatusCode = updateUserAttributesResponse.HttpStatusCode,
                    AccessToken = accessToken
                };

                return userAttributeResponse;

                //return updateUserAttributesResponse;

            }
            catch (AggregateException e)
            {
                var userAttributeResponse = new UserAttributeResponse
                {
                    AccessToken = accessToken,
                    Message = e.Message,
                    Exception = e.InnerException == null ? string.Empty : e.InnerException.ToString()
                };
                return userAttributeResponse;
                //throw e;
            }
            catch (Exception e)
            {
                var userAttributeResponse = new UserAttributeResponse
                {
                    AccessToken = accessToken,
                    Message = e.Message,
                    Exception = e.InnerException == null ? string.Empty : e.InnerException.ToString()
                };
                return userAttributeResponse;

            }
        }

        public static UserAttributeResponse VerifyUserEmailAttribute(string verificationCode, string accessToken, string attributeName)
        {
            AmazonCognitoIdentityProviderClient provider =
                                         new AmazonCognitoIdentityProviderClient(_awsCredentials, Region);
            try
            {
                var verifyResponse = provider.VerifyUserAttribute(new VerifyUserAttributeRequest
                {
                    AttributeName = attributeName,
                    AccessToken = accessToken,
                    Code = verificationCode
                });

                var userAttributeResponse = new UserAttributeResponse
                {
                    HttpStatusCode = verifyResponse.HttpStatusCode,
                    AccessToken = accessToken
                };

                return userAttributeResponse;
            }
           
            catch (AggregateException e)
            {
                var userAttributeResponse = new UserAttributeResponse
                {
                    AccessToken = accessToken,
                    Message = e.Message,
                    Exception = e.InnerException == null ? string.Empty : e.InnerException.ToString()
                };
                return userAttributeResponse;
                //throw e;
            }
            catch (Exception e)
            {
                var userAttributeResponse = new UserAttributeResponse
                {
                    AccessToken = accessToken,
                    Message = e.Message,
                    Exception = e.InnerException == null ? string.Empty : e.InnerException.ToString()
                };
                return userAttributeResponse;

            }
        }
    }

    public class CognitoServiceException : Exception
    {
        public CognitoServiceException(string failedToRespondToCognitoTwoFactorChallenge, AmazonCognitoIdentityProviderException amazonCognitoIdentityProviderException)
        {
            throw new NotImplementedException();
        }
    }


    //     public virtual async Task<AuthFlowResponse> RespondToTwoFactorChallengeAsync(string email, string code, string authWorkflowSessionId, CancellationToken cancellationToken)
    //{
    //    cancellationToken.ThrowIfCancellationRequested();

    //    string[] emailUsername = email.Split('@');

    //    CognitoUserPool userPool = new CognitoUserPool(PoolId, ClientAppId, _provider);

    //    CognitoUser user = new CognitoUser("4caa6265-358f-4637-9e17-78da7dad8ab1", ClientAppId, userPool, _provider);

    //    try
    //    {
    //        AuthFlowResponse context =
    //            await user.RespondToSmsMfaAuthAsync(new RespondToSmsMfaRequest()
    //            {
    //                SessionID = authWorkflowSessionId.Trim(),
    //                MfaCode = code.Trim()
    //            }).ConfigureAwait(false);

    //        return context;


    //    }
    //    catch (AmazonCognitoIdentityProviderException e)

    //    {
    //        //throw new CognitoServiceException("Failed to respond to Cognito two factor challenge.", e);
    //        throw e;
    //    }
    //}

    //public static async Task<string> GetCredentialsAsync(string email, string password)
    //{
    //    AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(AwsConfiguration.CognitoAccessKeyId, AwsConfiguration.CognitoSecretAccessKey,Region);
    //    //AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), Region);

    //    CognitoUserPool userPool = new CognitoUserPool(PoolId, ClientAppId, provider);

    //   // string[] emailUsername = email.Split('@');

    //    CognitoUser user = new CognitoUser(email, ClientAppId, userPool, provider);

    //    InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
    //    {
    //        Password = password

    //    };
    //    try
    //    {
    //        AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);

    //        _sessionId = authResponse.SessionID;

    //        return _sessionId;
    //    }
    //    catch (Exception e)
    //    {
    //        throw e;
    //    }
    //}


}