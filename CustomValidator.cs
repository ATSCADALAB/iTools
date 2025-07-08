using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace ATSCADA.iWinTools
{
    public class CustomValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            if ((userName != Account.UserName) || (password != Account.Password))
            {
                throw new SecurityTokenException("Validation Failed!");
            }
        }
    }
}
