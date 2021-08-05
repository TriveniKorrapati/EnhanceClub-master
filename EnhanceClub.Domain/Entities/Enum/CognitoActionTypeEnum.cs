using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities.Enum
{
    public enum CognitoActionTypeEnum
    {
        SignIn = 1,
        SignUp ,
        EditAccount,
        Checkout,
        ResetPassword,
        VerifyResetPassword,
        PartialSignUp,
        UserRequestedEmailResend,
        InCompleteSignup
    }
}
