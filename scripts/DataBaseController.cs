using System.Linq;
using Godot;


public partial class DataBaseController : Node
{
    private GameContext context = new();
    private int currentScore = 0;
    public void InsertPlayer(PlayerInfo player)
    {
        if (context.Players.Any(x => x.Name == player.Name))
        {
            return;
        }
        var currentPlayer = new PlayerContext
        {
            Name = player.Name,
        };
        context.Players.Add(currentPlayer);
        context.SaveChanges();
    }
    
    public void UpdateHighScore(PlayerInfo player)
    {
        var currentPlayer = context.Players.Where(x => x.Name == player.Name).First();
        currentScore = player.Score;
        if (currentScore > currentPlayer.HighScore)
        {
            currentPlayer.HighScore = currentScore;
        }
        context.SaveChanges();
    }
    public void UpdateTotalScores(PlayerInfo player1, PlayerInfo player2)
    {
        var currentPlayer1 = context.Players.Where(x => x.Name == player1.Name).First();
        var currentPlayer2 = context.Players.Where(x => x.Name == player2.Name).First();
        currentScore = player1.Score;
        currentPlayer1.TotalScore += currentScore;
        currentScore = player2.Score;
        currentPlayer2.TotalScore += currentScore;
        context.SaveChanges();
    }
}