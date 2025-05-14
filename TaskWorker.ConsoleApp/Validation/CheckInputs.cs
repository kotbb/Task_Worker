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
    
}