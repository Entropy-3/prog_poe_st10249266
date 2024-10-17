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

        // Insert a new claim into the database
        public int insert_Claim(ClaimTBL m)
        {
            string sql = "INSERT INTO tblClaims (userID, fileURL) VALUES (@UserID, @FileURL)";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@UserID", m.userID);
            cmd.Parameters.AddWithValue("@FileURL", m.fileURL);
            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            return rowsAffected;
        }

        // Select all claims from database based on userID using an object list to store the results
        public List<ClaimTBL> SelectClaims(int userID)
        {
            List<ClaimTBL> claims = new List<ClaimTBL>();
            using (SqlConnection con = new SqlConnection(con_string))
            {
                string sql = "SELECT claimID, userID, fileURL FROM tblClaims WHERE userID = @UserID";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@UserID", userID);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ClaimTBL claim = new ClaimTBL();
                        claim.claimID = reader.GetInt32(0);
                        claim.userID = reader.GetInt32(1);
                        claim.fileURL = reader.GetString(2);
                        claims.Add(claim);
                    }
                }
            }
            return claims;
        }   

    }
}
