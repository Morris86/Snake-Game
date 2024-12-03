using System.Net.Sockets;
using System.Net;
using System.Text;
using MySql.Data.MySqlClient;

public class SnakeWebServer
{
    private const string connectionString = "server=atr.eng.utah.edu;" +
                                            "database=u1495290;" +
                                            "uid=u1495290;" +
                                            "password=BugSquashers;";

    public static void Main(string[] args)
    {
        TcpListener listener = new TcpListener(IPAddress.Any, 8080);
        listener.Start();
        Console.WriteLine("Web Server started on port 8080...");

        while (true)
        {
            using TcpClient client = listener.AcceptTcpClient();
            HandleClient(client);
        }
    }

    private static void HandleClient(TcpClient client)
    {
        using var networkStream = client.GetStream();
        using var reader = new StreamReader(networkStream, new UTF8Encoding(false));
        using var writer = new StreamWriter(networkStream, new UTF8Encoding(false)) { AutoFlush = true };

        // Read the request
        string request = reader.ReadLine();
        if (string.IsNullOrEmpty(request)) return;

        // Parse the request
        string[] tokens = request.Split(' ');
        if (tokens.Length < 2) return;

        string method = tokens[0];
        string path = tokens[1];

        // Route the request
        if (method == "GET")
        {
            if (path == "/")
            {
                ServeHomePage(writer);
            }
            else if (path == "/games")
            {
                ServeGamesPage(writer);
            }
            else if (path.StartsWith("/games?gid="))
            {
                string gameIdStr = path.Substring("/games?gid=".Length);
                if (int.TryParse(gameIdStr, out int gameId))
                {
                    ServeGameStatsPage(writer, gameId);
                }
                else
                {
                    Serve404Page(writer);
                }
            }
            else if (path.StartsWith("/players?pid=")) // New route for player stats
            {
                string playerIdStr = path.Substring("/players?pid=".Length);
                if (int.TryParse(playerIdStr, out int playerId))
                {
                    ServePlayerStatsPage(writer, playerId); // New function to serve player stats
                }
                else
                {
                    Serve404Page(writer);
                }
            }
            else
            {
                Serve404Page(writer);
            }
        }
        else
        {
            Serve404Page(writer);
        }

    }

    private static void ServeHomePage(StreamWriter writer)
    {
        string html = "<html><h3>Welcome to the Snake Games Database!</h3><a href=\"/games\">View Games</a></html>";
        SendResponse(writer, html);
    }

    private static void ServeGamesPage(StreamWriter writer)
    {
        using var conn = new MySqlConnection(connectionString);
        conn.Open();

        string query = "SELECT ID, StartTime, EndTime FROM Games";
        using var cmd = new MySqlCommand(query, conn);
        using var reader = cmd.ExecuteReader();

        var html = new StringBuilder();
        html.AppendLine("<html>");
        html.AppendLine("<h3>All Games</h3>");
        html.AppendLine("<table border=\"1\"><thead><tr><td>ID</td><td>Start</td><td>End</td></tr></thead><tbody>");
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string start = reader.GetDateTime(1).ToString();
            string end = reader.IsDBNull(2) ? "In Progress" : reader.GetDateTime(2).ToString();
            html.AppendLine($"<tr><td><a href=\"/games?gid={id}\">{id}</a></td><td>{start}</td><td>{end}</td></tr>");
        }
        html.AppendLine("</tbody></table>");
        html.AppendLine("<br><a href=\"/\">Back to Home</a>"); // Add the back link
        html.AppendLine("</html>");

        SendResponse(writer, html.ToString());
    }


    private static void ServeGameStatsPage(StreamWriter writer, int gameId)
    {
        using var conn = new MySqlConnection(connectionString);
        conn.Open();

        string query = "SELECT ID, Name, MaxScore, EnterTime, LeaveTime FROM Players WHERE GameID = @GameID";
        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@GameID", gameId);
        using var reader = cmd.ExecuteReader();

        var html = new StringBuilder();
        html.AppendLine($"<html>");
        html.AppendLine($"<h3>Stats for Game {gameId}</h3>");
        html.AppendLine("<table border=\"1\"><thead>" +
                        "<tr><td>Player ID</td><td>Player Name</td><td>Max Score</td><td>Enter Time</td><td>Leave Time</td></tr></thead><tbody>");
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);
            int maxScore = reader.GetInt32(2);
            string enterTime = reader.GetDateTime(3).ToString();
            string leaveTime = reader.IsDBNull(4) ? "In Progress" : reader.GetDateTime(4).ToString();
            html.AppendLine($"<tr><td><a href=\"/players?pid={id}\">{id}</a></td><td>{name}</td><td>{maxScore}</td><td>{enterTime}</td><td>{leaveTime}</td></tr>");
        }
        html.AppendLine("</tbody></table>");
        html.AppendLine($"<br><a href=\"/games\">Back to Games</a>");
        html.AppendLine("</html>");

        SendResponse(writer, html.ToString());
    }


    private static void ServePlayerStatsPage(StreamWriter writer, int playerId)
    {
        using var conn = new MySqlConnection(connectionString);
        conn.Open();

        string query = @"
        SELECT Players.ID, Players.Name, Players.MaxScore, Games.ID AS GameID, Games.StartTime, Games.EndTime
        FROM Players
        JOIN Games ON Players.GameID = Games.ID
        WHERE Players.ID = @PlayerID";
        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@PlayerID", playerId);
        using var reader = cmd.ExecuteReader();

        var html = new StringBuilder();
        html.AppendLine($"<html>");
        html.AppendLine($"<h3>Stats for Player {playerId}</h3>");
        html.AppendLine("<table border=\"1\"><thead>" +
                        "<tr><td>Game ID</td><td>Player Name</td><td>Max Score</td><td>Game Start</td><td>Game End</td></tr></thead><tbody>");
        while (reader.Read())
        {
            int gameId = reader.GetInt32(3);
            string name = reader.GetString(1);
            int maxScore = reader.GetInt32(2);
            string startTime = reader.GetDateTime(4).ToString();
            string endTime = reader.IsDBNull(5) ? "In Progress" : reader.GetDateTime(5).ToString();
            html.AppendLine($"<tr><td><a href=\"/games?gid={gameId}\">{gameId}</a></td><td>{name}</td><td>{maxScore}</td><td>{startTime}</td><td>{endTime}</td></tr>");
        }
        html.AppendLine("</tbody></table>");
        html.AppendLine($"<br><a href=\"/games\">Back to Games</a>");
        html.AppendLine("</html>");

        SendResponse(writer, html.ToString());
    }


    private static void Serve404Page(StreamWriter writer)
    {
        string html = "<html><h3>404 - Page Not Found</h3></html>";
        SendResponse(writer, html);
    }

    private static void SendResponse(StreamWriter writer, string html)
    {
        byte[] htmlBytes = Encoding.UTF8.GetBytes(html);
        string header = "HTTP/1.1 200 OK\r\n" +
                        "Connection: close\r\n" +
                        "Content-Type: text/html; charset=UTF-8\r\n" +
                        $"Content-Length: {htmlBytes.Length}\r\n" +
                        "\r\n";
        writer.Write(header);
        writer.Write(html);
        writer.Flush();
    }

}
