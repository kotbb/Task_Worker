namespace TaskWorker.Validation;

public class CheckInputs
{
    public static bool isValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        
        int atSymbolIndex = email.IndexOf('@');
        if (atSymbolIndex <= 0 || atSymbolIndex == email.Length - 1)
            return false;
        
        if (email.Contains(" ") || email.Contains(".."))
            return false;
        
        return true;
    }
    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        // Check if null or empty
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // Check length requirement (6-15 digits)
        if (phoneNumber.Length < 6 || phoneNumber.Length > 15)
            return false;

        // Check if all characters are digits
        foreach (char c in phoneNumber)
        {
            if (!char.IsDigit(c))
                return false;
        }

        return true;
    }
}