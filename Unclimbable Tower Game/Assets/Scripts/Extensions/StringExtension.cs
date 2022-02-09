public static class StringExtension {

    // Returns all numbers in string
    public static int? FetchNumber(this string text) {
        string number = string.Empty;
        for (int i = 0; i < text.Length; i++) {
            if (char.IsDigit(text[i]))
                number += text[i];
        }

        if (number.Length > 0)
            return int.Parse(number);
        else
            return null;
    }
}
