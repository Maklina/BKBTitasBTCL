using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BKBTitas.Models.AppManager
{
    public class BTCLTokenManager
    {
        private string AuthToken = null;
        private string error = null;
        private DateTime? lastTokenGenerated = null;

        public string GetToken(SvcRef_BTCLBoss.tAuthHeader header, SvcRef_BTCLBoss.tTokenRequest request, out string outErr)
        {
            if (AuthToken == null || IsTokenExpired())
            {
                GenerateTokenBoss(header, request);
            }
            outErr = error;
            return AuthToken;
        }

        private void GenerateTokenBoss(SvcRef_BTCLBoss.tAuthHeader header, SvcRef_BTCLBoss.tTokenRequest request)
        {
            string message;
            long ResponseCode;
            SvcRef_BTCLBoss.CrmWebServicesPortTypeClient client = new SvcRef_BTCLBoss.CrmWebServicesPortTypeClient();
            string token = client.Token(header, request, out ResponseCode, out message);
            if (ResponseCode == 0)
            {
                AuthToken = token;
                lastTokenGenerated = DateTime.Now;
            }
            else
            {
                error = ResponseCode.ToString() + " " + message;
            }
        }


        private void GenerateToken(string userID, string password)
        {
            SvcRef_BTCL.SLMS_ServiceClient client = new SvcRef_BTCL.SLMS_ServiceClient();
            SvcRef_BTCL.TokenRequest token = client.GetTokenByUser(userID, password);
            if (token.ResponseCode == 0)
            {
                AuthToken = token.TokenNumber;
                lastTokenGenerated = DateTime.Now;
            }else
            {
                error = token.ResponseCode.ToString()+" "+ token.Message;
            }
        }

        private bool IsTokenExpired()
        {
            System.DateTime dtDateTime = Convert.ToDateTime(lastTokenGenerated).AddHours(24);
            //dtDateTime = dtDateTime.AddSeconds(AuthToken.expires_in).ToLocalTime();
            return dtDateTime <= DateTime.Now;
        }
    }
}