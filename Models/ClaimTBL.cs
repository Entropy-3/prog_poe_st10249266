using System.Data.SqlClient;

namespace prog_poe_st10249266.Models
{
    public class ClaimTBL
    {

        public static string con_string = "Server=tcp:st10249266-sql-server.database.windows.net,1433;Initial Catalog=CLDV-DBS;Persist Security Info=False;User ID=entropy-3;Password=hifr@220404;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public static SqlConnection con = new SqlConnection(con_string);

        public int claimID { get; set; }
        public int userID { get; set; }
        public string fileURL { get; set; }
        public int hoursWorked { get; set; }
        public int hourlyrate { get; set; }
        public int amountDue { get; set; }
        public string claimStatus { get; set; }

        // Insert a new claim into the database
        public int insert_Claim(ClaimTBL m)
        {
            string sql = "INSERT INTO tblClaims (user_id, fileURL, hoursWorked, hourlyrate, amountDue, claimStatus) VALUES (@UserID, @FileURL, @HoursWorked, @HourlyRate, @AmountDue, @ClaimStatus)";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@UserID", m.userID);
            cmd.Parameters.AddWithValue("@FileURL", m.fileURL);
            cmd.Parameters.AddWithValue("@HoursWorked", m.hoursWorked);
            cmd.Parameters.AddWithValue("@HourlyRate", m.hourlyrate);
            cmd.Parameters.AddWithValue("@AmountDue", m.amountDue);
            cmd.Parameters.AddWithValue("@ClaimStatus", m.claimStatus);
            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            return rowsAffected;
        }

        // Select all claims from database based on userID using an object list to store the results
        public static List<ClaimTBL> GetClaims(int userID)
        {
            List<ClaimTBL> claims = new List<ClaimTBL>();

            string sql = "SELECT claim_id, user_id, fileURL, hoursWorked, hourlyrate, amountDue, claimStatus FROM tblClaims WHERE user_id = @userID";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@userID", userID);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ClaimTBL claim = new ClaimTBL();
                claim.claimID = Convert.ToInt32(reader["claim_id"]);
                claim.userID = Convert.ToInt32(reader["user_id"]);
                claim.fileURL = Convert.ToString(reader["fileURL"]);
                claim.hoursWorked = Convert.ToInt32(reader["hoursWorked"]);
                claim.hourlyrate = Convert.ToInt32(reader["hourlyrate"]);
                claim.amountDue = Convert.ToInt32(reader["amountDue"]);
                claim.claimStatus = Convert.ToString(reader["claimStatus"]);
                claims.Add(claim);
            }
            reader.Close();
            con.Close();

            return claims;
        }

    }
}
