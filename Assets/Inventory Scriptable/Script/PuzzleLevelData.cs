[System.Serializable]
public class PuzzleLevelData
{
    public int Id;
    public int isCompleted;
    public float bestTime;
    
    public PuzzleLevelData(int id, int iscompleted, float bestime) {
        this.Id = id;
        this.isCompleted = iscompleted;
        this.bestTime = bestime;
    }
}