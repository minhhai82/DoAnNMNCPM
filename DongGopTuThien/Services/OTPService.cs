using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using Twilio.Types;

public class TwilioConfig
{
    public string AccountSid { get; set; }
    public string AuthToken { get; set; }
    public string ServiceId { get; set; }
}
public interface IOTPService
{
    Task<bool> SendOtp(string phoneNumber);
    Task<bool> VerifyOtp(string phoneNumber, string code);
}

public class OTPService : IOTPService
{
    private readonly string _twilioAccountSid;
    private readonly string _twilioAuthToken;

    private readonly string _twilioServiceId;

    public OTPService(string twilioAccountSid, string twilioAuthToken, string twilioServiceId)
    {
        _twilioAccountSid = twilioAccountSid;
        _twilioAuthToken = twilioAuthToken;
        _twilioServiceId = twilioServiceId;
        
        TwilioClient.Init(_twilioAccountSid, _twilioAuthToken);
    }

    public async Task<bool> SendOtp(string phoneNumber)
    {
        // Send OTP via SMS
        try
        {
            var verification = await VerificationResource.CreateAsync(
               to: phoneNumber,
               channel: "sms",
               pathServiceSid: _twilioServiceId);

            if (verification.Status == "pending")
            {
                return true;
            }
            return false;
        } catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> VerifyOtp(string phoneNumber, string code)
    {

        var verificationCheck = await VerificationCheckResource.CreateAsync(
            to: phoneNumber,
            code: code,
            pathServiceSid: _twilioServiceId);


        Console.WriteLine(verificationCheck.Status);

        if (verificationCheck.Status == "approved")
        {
            return true;
        }

        return false;
    }
}
