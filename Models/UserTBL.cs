using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace prog_poe_st10249266.Models
{
    public class UserTBL
    {
        public static string con_string = "Server=tcp:st10249266-sql-server.database.windows.net,1433;Initial Catalog=CLDV-DBS;Persist Security Info=False;User ID=entropy-3;Password=hifr@220404;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static SqlConnection con = new SqlConnection(con_string);

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool isAdmin { get; set; }

        public int insert_User(UserTBL m)
        {
            string sql = "INSERT INTO tblUsers (userName, userSurname, userEmail, userPassword, isAdmin) VALUES (@Name, @Surname, @Email, @Password, @Admin)";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Name", m.Name);
            cmd.Parameters.AddWithValue("@Surname", m.Surname);
            cmd.Parameters.AddWithValue("@Email", m.Email);
            cmd.Parameters.AddWithValue("@Password", m.Password);
            cmd.Parameters.AddWithValue("@Admin", m.isAdmin ? 1 : 0);
            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            return rowsAffected;
        }
    }
}
