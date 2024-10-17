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
        public string claimStatus { get; set; }

        // Insert a new claim into the database
        public int insert_Claim(ClaimTBL m)
        {
            string sql = "INSERT INTO tblClaims (userID, fileURL, hoursWorked, hourlyrate, claimStatus) VALUES (@UserID, @FileURL, @HoursWorked, @HourlyRate, @ClaimStatus)";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@UserID", m.userID);
            cmd.Parameters.AddWithValue("@FileURL", m.fileURL);
            cmd.Parameters.AddWithValue("@HoursWorked", m.hoursWorked);
            cmd.Parameters.AddWithValue("@HourlyRate", m.hourlyrate);
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

            string sql = "SELECT claimID, userID, fileURL, hoursWorked, hourlyrate, claimStatus FROM tblClaims WHERE userID = @userID";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@userID", userID);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ClaimTBL claim = new ClaimTBL();
                claim.claimID = Convert.ToInt32(reader["claimID"]);
                claim.userID = Convert.ToInt32(reader["userID"]);
                claim.fileURL = Convert.ToString(reader["fileURL"]);
                claim.hoursWorked = Convert.ToInt32(reader["hoursWorked"]);
                claim.hourlyrate = Convert.ToInt32(reader["hourlyrate"]);
                claim.claimStatus = Convert.ToString(reader["claimStatus"]);
                claims.Add(claim);
            }
            reader.Close();
            con.Close();

            return claims;
        }

    }
}
