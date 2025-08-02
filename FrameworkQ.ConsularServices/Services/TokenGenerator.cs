using System.Text;
using Npgsql;

namespace FrameworkQ.ConsularServices.Services;


public class TokenGenerator
{
    // --- Token Generation ---

    private readonly string _connectionString;
    public TokenGenerator(ConnectionProvider connectionProvider)
    {
        _connectionString = connectionProvider.GetConnectionString();
    }

    /// <summary>
    /// Creates a unique token ID for a given date.
    /// The final token is 3 characters long: YMD
    /// </summary>
    /// <param name="date">The date for which to generate the token.</param>
    /// <returns>A string representing the generated token.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the year or day is outside the valid range.
    /// </exception>
    public string GenerateToken(DateTime date)
    {
        var tokenIdBuilder = new StringBuilder();
        tokenIdBuilder.Append(GetYearChar(date.Year));
        tokenIdBuilder.Append(GetMonthChar(date.Month));
        tokenIdBuilder.Append(GetDayChar(date.Day));
        return tokenIdBuilder.ToString();
    }

    // --- Token Degeneration ---

    /// <summary>
    /// Converts a 3-character token back into its original DateTime object.
    /// Note: Time components (Hour, Minute, Second) will be set to their default value (midnight).
    /// </summary>
    /// <param name="token">The 3-character token (YMD).</param>
    /// <returns>A DateTime object representing the date encoded in the token.</returns>
    /// <exception cref="ArgumentException">Thrown if the token is not valid.</exception>
    public DateTime DegenerateToken(string token)
    {
        if (string.IsNullOrEmpty(token) || token.Length != 3)
        {
            throw new ArgumentException("Token must be a 3-character string.", nameof(token));
        }

        try
        {
            int year = GetCharYear(token[0]);
            int month = GetCharMonth(token[1]);
            int day = GetCharDay(token[2]);

            return new DateTime(year, month, day);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            // Catch exceptions from the helper methods for invalid characters
            throw new ArgumentException("Token contains invalid characters.", ex);
        }
    }

    // --- Private Helpers for Generation ---

    private char GetYearChar(int year)
    {
        switch (year)
        {
            case 2025: return '5'; case 2026: return '6'; case 2027: return '7';
            case 2028: return '8'; case 2029: return '9'; case 2030: return '0';
            case 2031: return '1'; case 2032: return '2'; case 2033: return '3';
            case 2034: return '4'; case 2035: return 'A'; case 2036: return 'B';
            case 2037: return 'C'; case 2038: return 'D'; case 2039: return 'E';
            case 2040: return 'F';
            default: throw new ArgumentOutOfRangeException(nameof(year), "Year must be between 2025 and 2040.");
        }
    }

    private char GetMonthChar(int month)
    {
        switch (month)
        {
            case 1: return '1'; case 2: return '2'; case 3: return '3';
            case 4: return '4'; case 5: return '5'; case 6: return '6';
            case 7: return '7'; case 8: return '8'; case 9: return '9';
            case 10: return '0'; case 11: return 'A'; case 12: return 'B';
            default: throw new ArgumentOutOfRangeException(nameof(month), "Invalid month.");
        }
    }
    
    private char GetDayChar(int day)
    {
        if (day >= 1 && day <= 9) return (char)(day + '0');
        switch (day)
        {
            case 10: return '0'; case 11: return 'A'; case 12: return 'B';
            case 13: return 'C'; case 14: return 'D'; case 15: return 'E';
            case 16: return 'F'; case 17: return 'G'; case 18: return 'H';
            case 19: return 'I'; case 20: return 'J'; case 21: return 'K';
            case 22: return 'L'; case 23: return 'M'; case 24: return 'N';
            case 25: return 'O'; case 26: return 'P'; case 27: return 'Q';
            case 28: return 'R'; case 29: return 'S'; case 30: return 'T';
            case 31: return 'U';
            default: throw new ArgumentOutOfRangeException(nameof(day), "Day must be between 1 and 31.");
        }
    }

    // --- Private Helpers for Degeneration ---

    private int GetCharYear(char c)
    {
        switch (c)
        {
            case '5': return 2025; case '6': return 2026; case '7': return 2027;
            case '8': return 2028; case '9': return 2029; case '0': return 2030;
            case '1': return 2031; case '2': return 2032; case '3': return 2033;
            case '4': return 2034; case 'A': return 2035; case 'B': return 2036;
            case 'C': return 2037; case 'D': return 2038; case 'E': return 2039;
            case 'F': return 2040;
            default: throw new ArgumentOutOfRangeException(nameof(c), "Invalid character for Year.");
        }
    }

    private int GetCharMonth(char c)
    {
        switch (c)
        {
            case '1': return 1; case '2': return 2; case '3': return 3;
            case '4': return 4; case '5': return 5; case '6': return 6;
            case '7': return 7; case '8': return 8; case '9': return 9;
            case '0': return 10; case 'A': return 11; case 'B': return 12;
            default: throw new ArgumentOutOfRangeException(nameof(c), "Invalid character for Month.");
        }
    }

    private int GetCharDay(char c)
    {
        if (c >= '1' && c <= '9') return c - '0';
        switch (c)
        {
            case '0': return 10; case 'A': return 11; case 'B': return 12;
            case 'C': return 13; case 'D': return 14; case 'E': return 15;
            case 'F': return 16; case 'G': return 17; case 'H': return 18;
            case 'I': return 19; case 'J': return 20; case 'K': return 21;
            case 'L': return 22; case 'M': return 23; case 'N': return 24;
            case 'O': return 25; case 'P': return 26; case 'Q': return 27;
            case 'R': return 28; case 'S': return 29; case 'T': return 30;
            case 'U': return 31;
            default: throw new ArgumentOutOfRangeException(nameof(c), "Invalid character for Day.");
        }
    }
    
    /// <summary>
    /// Generates a token with a sequence number from the database.
    /// The final token is 6 characters long: YMDNNN
    /// </summary>
    /// <param name="connectionString">The connection string to the PostgreSQL database.</param>
    /// <param name="sequencePrefix">A prefix to distinguish different sequence types (e.g., "INV", "ORD").</param>
    /// <param name="date">The date for the token.</param>
    /// <returns>A 6-character token string.</returns>
    public string GenerateTokenWithSequence( string sequencePrefix, DateTime date)
    {
        // 1. Generate the 3-character date part of the token
        string datePart = GenerateToken(date);

        // 2. Get the next sequence number from the database
        long sequenceNo = GetNextDbSequence(sequencePrefix, datePart);

        if (sequenceNo > 999)
        {
            throw new InvalidOperationException("Sequence number exceeds the maximum value of 999.");
        }

        // 3. Combine the parts into the final token
        // Token is YMD (from datePart) + NNN (3-digit sequence)
        return $"{datePart}{sequenceNo:D3}";
    }

    /// <summary>
    /// Calls the database function to get the next available sequence number.
    /// </summary>
    private long GetNextDbSequence( string prefix, string datePart)
    {
        long nextSequence = -1;

        // Use Npgsql to connect to your PostgreSQL database
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            conn.Open();

            // Create a command to execute the database function
            using (var cmd = new NpgsqlCommand("SELECT public.get_next_sequence_no(@p_prefix, @p_date_part)", conn))
            {
                // Add parameters to prevent SQL injection
                cmd.Parameters.AddWithValue("p_prefix", prefix);
                cmd.Parameters.AddWithValue("p_date_part", datePart);

                // Execute the function and get the result
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    nextSequence = Convert.ToInt64(result);
                }
            }
            
            // After getting the sequence, you would typically insert the new record
            // This part is crucial to "claim" the sequence number you just fetched.
            using (var insertCmd = new NpgsqlCommand(
                "INSERT INTO public.sequence (sequence_prefix, date_part, sequence_no, generated_at) VALUES (@p_prefix, @p_date_part, @p_seq_no, @p_timestamp)", conn))
            {
                insertCmd.Parameters.AddWithValue("p_prefix", prefix);
                insertCmd.Parameters.AddWithValue("p_date_part", datePart);
                insertCmd.Parameters.AddWithValue("p_seq_no", nextSequence);
                insertCmd.Parameters.AddWithValue("p_timestamp", DateTime.UtcNow);
                insertCmd.ExecuteNonQuery();
            }
        }

        if (nextSequence == -1)
        {
            throw new InvalidOperationException("Could not retrieve sequence number from the database.");
        }

        return nextSequence;
    }
}